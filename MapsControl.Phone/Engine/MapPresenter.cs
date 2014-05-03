using System;
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
using System.Reactive;
using System.Reactive.Linq;
#endif

namespace MapsControl.Engine
{
    public class MapPresenter : IMapPresenter
    {
        #region Consts

        private const int MaxLevelOfDetails = 17;
        private const int MinLevelOfDetails = 1;
        private const int TileSize = 256;

        #endregion

        #region Fields

        private readonly int _tileResolution;
        private readonly IList<Tile> _tiles = new List<Tile>();
        private readonly IList<Pin> _pins = new List<Pin>();
        private GeoCoordinate _geoCoordinateCenter;
        private double _levelOfDetail = 14;
        private ITileUriProvider _tileUriProvider;
        private Size _viewWindowsSize;
        private readonly IMapView _mapView;

        private readonly IList<MapOverlayPresenter> _mapOverlayPresenters = new List<MapOverlayPresenter>();

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

        private ITileUriProvider TileUriProvider
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
                Initialize();
            }
        }

        #endregion

        #region Constructor

        public MapPresenter(IMapView mapView, IMapCommands mapCommands, int tileResolution)
        {
            _tileResolution = tileResolution;
            TileUriProvider = new NullTileUriProvider();
            BuildTiles();

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
            Enumerable.Range(0, _tileResolution * _tileResolution)
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

            for (int x = 0; x < _tileResolution; ++x)
            {
                for (int y = 0; y < _tileResolution; ++y)
                {
                    var tile = _tiles[y + x * _tileResolution];
                    tile.MapX = pointInTileIndexes.X + x - _tileResolution / 2;
                    tile.MapY = pointInTileIndexes.Y + y - _tileResolution / 2;
                    tile.OffsetX = (x - _tileResolution / 2) * TileSize - pointInTileRelativePixels.X + _viewWindowsSize.Width / 2;
                    tile.OffsetY = (y - _tileResolution / 2) * TileSize - pointInTileRelativePixels.Y + _viewWindowsSize.Height / 2;
                    tile.Uri = TileUriProvider.GetTileUri((int)_levelOfDetail, tile.MapX, tile.MapY);
                }
            }

            _pins.ForEach(PositionPin);
        }

        private void MoveTiles(Point2D offset)
        {
            double minOffset = -1 * TileSize;  
            double maxOffset = TileSize * (_tileResolution - 1);

            foreach (var tile in _tiles)
            {
                tile.OffsetX += offset.X;
                tile.OffsetY += offset.Y;

                if (tile.OffsetX < minOffset)
                {
                    tile.OffsetX += _tileResolution * TileSize;
                    tile.MapX += _tileResolution;
                    tile.Uri = TileUriProvider.GetTileUri((int)_levelOfDetail, tile.MapX, tile.MapY);
                }
                else if (tile.OffsetX > maxOffset)
                {
                    tile.OffsetX -= _tileResolution * TileSize;
                    tile.MapX -= _tileResolution;
                    tile.Uri = TileUriProvider.GetTileUri((int)_levelOfDetail, tile.MapX, tile.MapY);
                }
                else if (tile.OffsetY < minOffset)
                {
                    tile.OffsetY += _tileResolution * TileSize;
                    tile.MapY += _tileResolution;
                    tile.Uri = TileUriProvider.GetTileUri((int)_levelOfDetail, tile.MapX, tile.MapY);
                }
                else if (tile.OffsetY > maxOffset)
                {
                    tile.OffsetY -= _tileResolution * TileSize;
                    tile.MapY -= _tileResolution;
                    tile.Uri = TileUriProvider.GetTileUri((int)_levelOfDetail, tile.MapX, tile.MapY);
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

        private void OnInitialized(EventArgs args)
        {
            foreach (var tile in _tiles)
            {
                var tileView = new ImageTileView(256);
                var tilePresenter = new TilePresenter(tileView, tile);

                _mapView.Add(tileView);
            }

            foreach (var mapOverlayPresenter in _mapOverlayPresenters)
            {
                _mapView.Add(mapOverlayPresenter.View);
            }
        }

        private void OnEntityViewAdded(IEnumerable<IMapEntityView> views)
        {
            foreach (var mapOverlay in views.OfType<IMapOverlayView>())
            {
                var pin = new Pin { GeoCoordinate = mapOverlay.GeoCoordinate };
                var mapOverlayPresenter = new MapOverlayPresenter(this, mapOverlay, pin);
            
                AddPin(pin);
                _mapOverlayPresenters.Add(mapOverlayPresenter);
            }
        }

        private void AddPin(Pin pin)
        {
            _pins.Add(pin);
        }

        public void PositionPin(Pin pin)
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