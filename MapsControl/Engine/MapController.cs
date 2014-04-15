using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Xml.Linq;
using MapsControl.Infrastructure;
using MapsControl.Presentation;
using MapsControl.TileUriProviders;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;

namespace MapsControl.Engine
{
    public class MapController : IMapController
    {
        #region Consts

        private const int MaxLevelOfDetails = 17;
        private const int MinLevelOfDetails = 1;

        #endregion

        #region Fields

        private readonly int _tileResolution;
        private readonly int _tileSize;
        private readonly IList<Tile> _tiles = new List<Tile>();
        private readonly IList<Pin> _pins = new List<Pin>();
        private GeoCoordinate _geoCoordinateCenter;
        private double _levelOfDetail = 14;
        private ITileUriProvider _tileUriProvider;
        private Size _viewWindowsSize;

        #endregion

        #region Properties

        public IEnumerable<Tile> Tiles { get { return _tiles; } }

        public int TileSize { get { return _tileSize; } }

        public int TileResolution { get { return _tileResolution; } }

        public GeoCoordinate GeoCoordinateCenter { get { return _geoCoordinateCenter; } }

        public double LevelOfDetail
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

        public ITileUriProvider TileUriProvider
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

        public Size ViewWindowSize
        {
            get
            {
                return _viewWindowsSize;
            }
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

        public MapController(int tileResolution, int tileSize)
        {
            _tileResolution = tileResolution;
            _tileSize = tileSize;
            TileUriProvider = new NullTileUriProvider();
            BuildTiles();
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

            Point2D pointInTileIndexes = _geoCoordinateCenter.ToPointInTileIndexes(_levelOfDetail, _tileSize);
            Point2D pointInTileRelativePixels = _geoCoordinateCenter.ToPointInTileRelativePixels(_levelOfDetail, _tileSize);

            for (int x = 0; x < _tileResolution; ++x)
            {
                for (int y = 0; y < _tileResolution; ++y)
                {
                    var tile = _tiles[y + x * _tileResolution];
                    tile.MapX = pointInTileIndexes.X + x - _tileResolution / 2;
                    tile.MapY = pointInTileIndexes.Y + y - _tileResolution / 2;
                    tile.OffsetX = (x - TileResolution / 2) * TileSize - pointInTileRelativePixels.X + _viewWindowsSize.Width / 2;
                    tile.OffsetY = (y - TileResolution / 2) * TileSize - pointInTileRelativePixels.Y + _viewWindowsSize.Height / 2;
                    tile.Uri = TileUriProvider.GetTileUri((int)_levelOfDetail, tile.MapX, tile.MapY);
                }
            }

            _pins.ForEach(PositionPin);
        }

        private void MoveTiles(Point2D offset)
        {
            double minOffset = -1 * _tileSize;  
            double maxOffset = _tileSize * (_tileResolution - 1);

            foreach (var tile in _tiles)
            {
                tile.OffsetX += offset.X;
                tile.OffsetY += offset.Y;

                if (tile.OffsetX < minOffset)
                {
                    tile.OffsetX += _tileResolution * _tileSize;
                    tile.MapX += _tileResolution;
                    tile.Uri = TileUriProvider.GetTileUri((int)_levelOfDetail, tile.MapX, tile.MapY);
                }
                else if (tile.OffsetX > maxOffset)
                {
                    tile.OffsetX -= _tileResolution * _tileSize;
                    tile.MapX -= _tileResolution;
                    tile.Uri = TileUriProvider.GetTileUri((int)_levelOfDetail, tile.MapX, tile.MapY);
                }
                else if (tile.OffsetY < minOffset)
                {
                    tile.OffsetY += _tileResolution * _tileSize;
                    tile.MapY += _tileResolution;
                    tile.Uri = TileUriProvider.GetTileUri((int)_levelOfDetail, tile.MapX, tile.MapY);
                }
                else if (tile.OffsetY > maxOffset)
                {
                    tile.OffsetY -= _tileResolution * _tileSize;
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

        public void AddPin(Pin pin)
        {
            _pins.Add(pin);
        }

        public void SetGeoCoordinateCenter(GeoCoordinate geoCoordinate)
        {
            _geoCoordinateCenter = geoCoordinate;
            Initialize();
        }

        public void Move(Point2D offset)
        {
            _geoCoordinateCenter = _geoCoordinateCenter.OffsetByPixels(offset, _levelOfDetail);
            MoveTiles(offset);
        }

        public void PositionPin(Pin pin)
        {
            if (pin.GeoCoordinate == null)
            {
                return;
            }

            Point2D pinWindowPoint = pin.GeoCoordinate.ToWindowPoint(
                _geoCoordinateCenter, _levelOfDetail, _viewWindowsSize);

            pin.OffsetX = pinWindowPoint.X;
            pin.OffsetY = pinWindowPoint.Y;
        }

        public void AddPolyline(PolylineEntity polylineEntity)
        {
            polylineEntity.Pins.ForEach(_pins.Add);
        }

        #endregion
    }
}