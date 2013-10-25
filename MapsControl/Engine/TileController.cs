using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Windows;

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
        private Point _tileWindowCenter = new Point();
        private GeoCoordinate _geoCoordinateCenter;
        private int _levelOfDetail = 14;

        #endregion

        #region Properties

        public IEnumerable<Tile> Tiles { get { return _tiles; } }

        public int TileSize { get { return _tileSize; } }

        public int TileResolution { get { return _tileResolution; } }

        public Point TileWindowCenter { get { return _tileWindowCenter; } }

        public GeoCoordinate GeoCoordinateCenter { get { return _geoCoordinateCenter; } }

        public int LevelOfDetail
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

        #endregion

        #region Constructor

        public TileController(int tileResolution, int tileSize)
        {
            _tileResolution = tileResolution;
            _tileSize = tileSize;
            BuildTiles();
            SetTileWindowCenter(_tileSize / 2, _tileSize / 2);
        }

        #endregion

        #region Methods

        private void BuildTiles()
        {
            for (int x = 0; x < _tileResolution; ++x)
            {
                for (int y = 0; y < _tileResolution; ++y)
                {
                    var tile = new Tile
                        {
                            X = x,
                            Y = y
                        };
                    _tiles.Add(tile);
                }
            }
        }

        private void PositionTiles()
        {
            int pixelY;
            int pixelX;

            _mapMath.LatLongToPixelXY(_geoCoordinateCenter.Latitude, _geoCoordinateCenter.Longitude, _levelOfDetail, out pixelX, out pixelY);

            int tileX = (int)Math.Floor(pixelX / (double)_tileSize);
            int tileY = (int)Math.Floor(pixelY / (double)_tileSize);

            for (int x = 0; x < _tileResolution; ++x)
            {
                for (int y = 0; y < _tileResolution; ++y)
                {
                    var tile = _tiles[y + x * _tileResolution];
                    tile.MapX = tileX + x - _tileResolution / 2;
                    tile.MapY = tileY + y - _tileResolution / 2;
                    tile.Uri = string.Format("http://{0}.tile.opencyclemap.org/cycle/{1}/{2}/{3}.png", "a", _levelOfDetail, tile.MapX, tile.MapY);
                }
            }

            int tilePixelX = pixelX % _tileSize;
            int tilePixelY = pixelY % _tileSize;

            SetTileWindowCenter(tilePixelX, tilePixelY);
        }

        private void SetTileWindowCenter(int tilePixelX, int tilePixelY)
        {
            int centerPixelX = (TileResolution / 2) * TileSize + tilePixelX;
            int centerPixelY = (TileResolution / 2) * TileSize + tilePixelY;
            _tileWindowCenter = new Point(centerPixelX, centerPixelY);
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
            _mapMath.LatLongToPixelXY(GeoCoordinateCenter.Latitude, GeoCoordinateCenter.Longitude, LevelOfDetail, out centerPixelX, out centerPixelY);

            centerPixelX -= deltaPixelX;
            centerPixelY -= deltaPixelY;

            double newCenterLatitute;
            double newCenterLongitude;
            _mapMath.PixelXYToLatLong(centerPixelX, centerPixelY, LevelOfDetail, out newCenterLatitute, out newCenterLongitude);

            var geoCoordinate = new GeoCoordinate(newCenterLatitute, newCenterLongitude);
            SetGeoCoordinateCenter(geoCoordinate);
        }

        #endregion
    }
}