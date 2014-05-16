using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Controls;
using MapsControl.Infrastructure;
using MapsControl.Presentation;
using MapsControl.TileUriProviders;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Windows;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif
#if DESKTOP
using System.Reactive.Linq;
using System.IO;
#endif

namespace MapsControl.Engine
{
    public enum XMapLayer
    {
        Tile = 0,
        Overlay = 1
    }

    public class MapPresenter
    {
        #region Consts

        private const int MaxLevelOfDetails = 17;
        private const int MinLevelOfDetails = 1;
        private const int TileSize = 256;

        #endregion

        #region Fields

        private readonly IList<Tile> _tiles = new List<Tile>();
        private readonly IList<Pin> _pins = new List<Pin>();
        private readonly IList<IMapOverlayView> _mapOverlayViews = new List<IMapOverlayView>();
        private readonly IMapView _mapView;
        private ITileLoader _tileLoader;
        private GeoCoordinate _geoCoordinateCenter;
        private Point2D _tileResolution;
        private double _levelOfDetail = 14;
        private ITileSourceProvider _tileUriProvider;
        private Size _viewWindowsSize;

        #endregion

        #region Properties

        private GeoCoordinate GeoCoordinateCenter
        {
            get
            {
                return _geoCoordinateCenter;
            }
            set
            {
                if (_geoCoordinateCenter == value)
                {
                    return;
                }
                _geoCoordinateCenter = value;
                Initialize();
            }
        }

        private double LevelOfDetail
        {
            get
            {
                return _levelOfDetail;
            }
            set
            {
                if (_levelOfDetail == value || value > MaxLevelOfDetails || value < MinLevelOfDetails)
                {
                    return;
                }
                _levelOfDetail = value;
                Initialize();
            }
        }

        private ITileSourceProvider TileUriProvider
        {
            get
            {
                return _tileUriProvider;
            }
            set
            {
                if (_tileUriProvider == value)
                {
                    return;
                }
                _tileUriProvider = value;
                _tileLoader = new TileLoader(_tileUriProvider);
                Initialize();
            }
        }

        private Size ViewWindowSize
        {
            get { return _viewWindowsSize; }

            set
            {
                if (_viewWindowsSize == value)
                {
                    return;
                }
                _viewWindowsSize = value;
                BuildTiles();
                InitializeTileViews();
                Initialize();
            }
        }

        #endregion

        #region Constructor

        public MapPresenter(IMapView mapView, IMapCommands mapCommands)
        {
            TileUriProvider = new NullTileUriProvider();

            _mapView = mapView;
            mapCommands.Translations.Where(translation => !translation.IsZero()).Subscribe(Move);
            mapCommands.SizeChanges.Subscribe(size => ViewWindowSize = size);
            mapCommands.Initialized.Take(1).Subscribe(OnInitialized);
            mapCommands.EntityViewAdded.Subscribe(OnEntityViewAdded);
            mapCommands.GeoCoordinateCenters.Subscribe(center => GeoCoordinateCenter = center);
            mapCommands.Zooms.Subscribe(zoom => LevelOfDetail = zoom);
            mapCommands.TileUriProviders.Subscribe(tileUriProvider => TileUriProvider = tileUriProvider);
        }

        #endregion

        #region Methods

        private void BuildTiles()
        {
            _tiles.Clear();
            int tileScaleX = (int)ViewWindowSize.Width / TileSize + 1;
            int tileScaleY = (int)ViewWindowSize.Height / TileSize + 1;
            _tileResolution = new Point2D(
                tileScaleX % 2 > 0 ? tileScaleX + 2 : tileScaleX + 1,
                tileScaleY % 2 > 0 ? tileScaleY + 2 : tileScaleY + 1);

            Enumerable.Range(0, _tileResolution.X * _tileResolution.Y)
                .Select(index => new Tile())
                .ToList()
                .ForEach(_tiles.Add);
        }

        private void Initialize()
        {
            if (_geoCoordinateCenter == null)
            {
                return;
            }

            Point2D pointInTileIndexes = _geoCoordinateCenter.ToPointInTileIndexes(_levelOfDetail, TileSize);
            Point2D pointInTileRelativePixels = _geoCoordinateCenter.ToPointInTileRelativePixels(_levelOfDetail, TileSize);

            if (_tiles.Any())
            {
                for (int x = 0; x < _tileResolution.X; ++x)
                {
                    for (int y = 0; y < _tileResolution.Y; ++y)
                    {
                        var tile = _tiles[y + x*_tileResolution.Y];
                        tile.MapX = pointInTileIndexes.X + x - _tileResolution.X/2;
                        tile.MapY = pointInTileIndexes.Y + y - _tileResolution.Y/2;
                        tile.LevelOfDetails = _levelOfDetail;

                        tile.Offset = new Point(
                            (x - _tileResolution.X/2)*TileSize - pointInTileRelativePixels.X + _viewWindowsSize.Width/2,
                            (y - _tileResolution.Y/2)*TileSize - pointInTileRelativePixels.Y + _viewWindowsSize.Height/2);

                        _tileLoader.LoadAsync(tile);
                    }
                }
            }

            _pins.ForEach(PositionPin);
        }

        private void MoveTiles(Point2D offset)
        {
            Debug.WriteLine("Move: {0}:{1}", offset.X, offset.Y);

            var minOffset = new Point2D(
                -1 * TileSize, -1 * TileSize);
            var maxOffset = new Point2D(
                TileSize * (_tileResolution.X - 1), TileSize * (_tileResolution.Y - 1));

            foreach (var tile in _tiles)
            {
                tile.Offset = tile.Offset.Move(offset);

                if (tile.Offset.X < minOffset.X)
                {
                    tile.Offset = tile.Offset.Move(_tileResolution.X * TileSize, 0);
                    tile.MapX += _tileResolution.X;
                    _tileLoader.LoadAsync(tile);
                }
                else if (tile.Offset.X > maxOffset.X)
                {
                    tile.Offset = tile.Offset.Move(-1*_tileResolution.X*TileSize, 0);
                    tile.MapX -= _tileResolution.X;
                    _tileLoader.LoadAsync(tile);
                }
                else if (tile.Offset.Y < minOffset.Y)
                {
                    tile.Offset = tile.Offset.Move(0, _tileResolution.Y * TileSize);
                    tile.MapY += _tileResolution.Y;
                    _tileLoader.LoadAsync(tile);
                }
                else if (tile.Offset.Y > maxOffset.Y)
                {
                    tile.Offset = tile.Offset.Move(0, -1 * _tileResolution.Y * TileSize);
                    tile.MapY -= _tileResolution.Y;
                    _tileLoader.LoadAsync(tile);
                }
            }

            foreach (var pin in _pins)
            {
                pin.Offset = pin.Offset.Move(offset);
            }
        }

        private void Move(Point2D offset)
        {
            _geoCoordinateCenter = _geoCoordinateCenter.OffsetByPixels(offset, _levelOfDetail);
            // Update geo coordinate center of map view.
            _mapView.GeoCoordinateCenter = _geoCoordinateCenter;
            MoveTiles(offset);
        }

        private readonly IList<ITileView> _tileViews = new List<ITileView>(); 

        private void InitializeTileViews()
        {
            _tileViews.ForEach(_mapView.Remove);

            foreach (var tile in _tiles)
            {
                ITileView tileView = new ImageTileView(256);

                tile.TileSourceChanges.StartWith(tile.TileSource)
                               .ObserveOn(SynchronizationContext.Current)
                               .Subscribe(tileSource => tileView.TileSource = tileSource);
                tile.OffsetChanges.StartWith(tile.Offset)
                               .ObserveOn(SynchronizationContext.Current)
                               .Subscribe(offset => tileView.Offset = offset);

                _mapView.Add(tileView, XMapLayer.Tile);
                _tileViews.Add(tileView);
            }
        }

        private void OnInitialized(EventArgs args)
        {
            InitializeTileViews();

            foreach (var mapOverlayView in _mapOverlayViews)
            {
                _mapView.Add(mapOverlayView, XMapLayer.Overlay);
            }
        }

        private void OnEntityViewAdded(IEnumerable<IMapEntityView> views)
        {
            foreach (var mapOverlay in views.OfType<IMapOverlayView>())
            {
                var pin = new Pin { GeoCoordinate = mapOverlay.GeoCoordinate };
                IMapOverlayView mapOverlayView = mapOverlay;

                PositionPin(pin);
                mapOverlay.GeoCoordinateChanged += (sender, coordinate) =>
                {
                    pin.GeoCoordinate = coordinate;
                    PositionPin(pin);
                };
                pin.OffsetChanges.StartWith(pin.Offset)
                                 .ObserveOn(SynchronizationContext.Current)
                                 .Subscribe(offset =>
                                 {
                                     mapOverlayView.Offset = new Point(
                                         offset.X,
                                         offset.Y - ((FrameworkElement)((ContentControl)mapOverlayView.VisualRoot).Content).Height);
                                 });

                AddPin(pin);
                _mapOverlayViews.Add(mapOverlayView);
            }
        }

        private void AddPin(Pin pin)
        {
            _pins.Add(pin);
        }

        private void PositionPin(Pin pin)
        {
            if (pin.GeoCoordinate == null || _geoCoordinateCenter == null)
            {
                return;
            }

            Point2D pinWindowPoint = pin.GeoCoordinate.ToWindowPoint(
                _geoCoordinateCenter, _levelOfDetail, _viewWindowsSize);

            pin.Offset = pinWindowPoint.ToPoint();
        }

        private void AddPolyline(PolylineEntity polylineEntity)
        {
            polylineEntity.Pins.ForEach(_pins.Add);
        }

        #endregion
    }
}