using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Xml.Linq;
using MapsControl.TileUriProviders;
using Microsoft.Phone.Maps.Controls;

namespace MapsControl.Engine
{
    public class TileController : ITileController
    {
        #region Consts

        private const int MaxLevelOfDetails = 17;
        private const int MinLevelOfDetails = 1;

        #endregion

        #region Fields

        private readonly MapMath _mapMath = new MapMath();
        private readonly int _tileResolution;
        private readonly int _tileSize;
        private readonly IList<Tile> _tiles = new List<Tile>();
        private Point _pixelCenter;
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
                PositionTiles();
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
                PositionTiles();
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
                PositionTiles();
            }
        }

        #endregion

        #region Constructor

        public TileController(int tileResolution, int tileSize)
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

        private void PositionTiles()
        {
            if (_geoCoordinateCenter == null)
            {
                return;
            }

            int pixelY;
            int pixelX;

            _mapMath.LatLongToPixelXY(_geoCoordinateCenter.Latitude, _geoCoordinateCenter.Longitude, (int)_levelOfDetail, out pixelX, out pixelY);

            int tileX = (int)Math.Floor(pixelX / (double)_tileSize);
            int tileY = (int)Math.Floor(pixelY / (double)_tileSize);

            _pixelCenter.X = pixelX;
            _pixelCenter.Y = pixelY;
            int tilePixelX = pixelX % _tileSize;
            int tilePixelY = pixelY % _tileSize;

            for (int x = 0; x < _tileResolution; ++x)
            {
                for (int y = 0; y < _tileResolution; ++y)
                {
                    var tile = _tiles[y + x * _tileResolution];
                    tile.MapX = tileX + x - _tileResolution / 2;
                    tile.MapY = tileY + y - _tileResolution / 2;
                    tile.OffsetX = (x - TileResolution / 2) * TileSize - tilePixelX + _viewWindowsSize.Width / 2;
                    tile.OffsetY = (y - TileResolution / 2) * TileSize - tilePixelY + _viewWindowsSize.Height / 2;
                    tile.Uri = TileUriProvider.GetTileUri((int)_levelOfDetail, tile.MapX, tile.MapY);
                }
            }
        }

        public void SetGeoCoordinateCenter(GeoCoordinate geoCoordinate)
        {
            _geoCoordinateCenter = geoCoordinate;
            PositionTiles();
        }

        public void Move(int deltaPixelX, int deltaPixelY)
        {
            int centerPixelX;
            int centerPixelY;
            _mapMath.LatLongToPixelXY(GeoCoordinateCenter.Latitude, GeoCoordinateCenter.Longitude, (int)LevelOfDetail, out centerPixelX, out centerPixelY);

            centerPixelX -= deltaPixelX;
            centerPixelY -= deltaPixelY;

            _pixelCenter.X = centerPixelX;
            _pixelCenter.Y = centerPixelY;

            double newCenterLatitute;
            double newCenterLongitude;
            _mapMath.PixelXYToLatLong(centerPixelX, centerPixelY, (int)LevelOfDetail, out newCenterLatitute, out newCenterLongitude);

            var geoCoordinate = new GeoCoordinate(newCenterLatitute, newCenterLongitude);
            _geoCoordinateCenter = geoCoordinate;
            MoveTiles(deltaPixelX, deltaPixelY);
            //SetGeoCoordinateCenter(geoCoordinate);
        }

        private void MoveTiles(int deltaPixelX, int deltaPixelY)
        {
            double minOffset = -1 * _tileSize;
            double maxOffset = _tileSize * (_tileResolution - 1);

            foreach (var tile in _tiles)
            {
                tile.OffsetX += deltaPixelX;
                tile.OffsetY += deltaPixelY;

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
        }

        public Point GetOffsetInPixelsRelativeToCenter(GeoCoordinate geoCoordinate)
        {
            int pixelY;
            int pixelX;

            _mapMath.LatLongToPixelXY(geoCoordinate.Latitude, geoCoordinate.Longitude, (int)_levelOfDetail, out pixelX, out pixelY);

            var offset = new Point(pixelX - _pixelCenter.X, pixelY - _pixelCenter.Y);
            return offset;
        }

        #endregion
    }
}