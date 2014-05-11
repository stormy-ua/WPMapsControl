using System;
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
            mapCommands.Translations.Subscribe(Move);
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
                        tile.OffsetX = (x - _tileResolution.X/2)*TileSize - pointInTileRelativePixels.X + _viewWindowsSize.Width/2;
                        tile.OffsetY = (y - _tileResolution.Y/2)*TileSize - pointInTileRelativePixels.Y + _viewWindowsSize.Height/2;
                        _tileLoader.LoadAsync(tile);
                    }
                }
            }

            _pins.ForEach(PositionPin);
        }

        private void MoveTiles(Point2D offset)
        {
            var minOffset = new Point2D(
                -1 * TileSize, -1 * TileSize);
            var maxOffset = new Point2D(
                TileSize * (_tileResolution.X - 1), TileSize * (_tileResolution.Y - 1));

            foreach (var tile in _tiles)
            {
                tile.OffsetX += offset.X;
                tile.OffsetY += offset.Y;

                if (tile.OffsetX < minOffset.X)
                {
                    tile.OffsetX += _tileResolution.X * TileSize;
                    tile.MapX += _tileResolution.X;
                    _tileLoader.LoadAsync(tile);
                }
                else if (tile.OffsetX > maxOffset.X)
                {
                    tile.OffsetX -= _tileResolution.X * TileSize;
                    tile.MapX -= _tileResolution.X;
                    _tileLoader.LoadAsync(tile);
                }
                else if (tile.OffsetY < minOffset.Y)
                {
                    tile.OffsetY += _tileResolution.Y * TileSize;
                    tile.MapY += _tileResolution.Y;
                    _tileLoader.LoadAsync(tile);
                }
                else if (tile.OffsetY > maxOffset.Y)
                {
                    tile.OffsetY -= _tileResolution.Y * TileSize;
                    tile.MapY -= _tileResolution.Y;
                    _tileLoader.LoadAsync(tile);
                }
            }

            foreach (var pin in _pins)
            {
                pin.OffsetX += offset.X;
                pin.OffsetY += offset.Y;
            }
        }

        private void Move(Point2D offset)
        {
            _geoCoordinateCenter = _geoCoordinateCenter.OffsetByPixels(offset, _levelOfDetail);
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
                tile.OffsetChanges.StartWith(new Point(tile.OffsetX, tile.OffsetY))
                               .Subscribe(offset => tileView.Offset = new Point(offset.X, offset.Y));

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
                pin.OffsetChanges.StartWith(new Point(pin.OffsetX, pin.OffsetY))
                                 .Subscribe(offset => mapOverlayView.Offset = new Point(offset.X, offset.Y));

                //_mapEntityView.OffsetY -= ((FrameworkElement)((ContentControl)_mapEntityView.VisualRoot).Content).Height;
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

            pin.OffsetX = pinWindowPoint.X;
            pin.OffsetY = pinWindowPoint.Y;
        }

        private void AddPolyline(PolylineEntity polylineEntity)
        {
            polylineEntity.Pins.ForEach(_pins.Add);
        }

        #endregion
    }
}