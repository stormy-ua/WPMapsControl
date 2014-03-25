using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Net;
using System.Net.Http;
using System.Windows;
using MapsControl.TileUriProviders;

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
        private Point _tileWindowCenter;
        private Point _pixelCenter;
        private GeoCoordinate _geoCoordinateCenter;
        private int _levelOfDetail = 14;
        private ITileUriProvider _tileUriProvider;

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

        #endregion

        #region Constructor

        public TileController(int tileResolution, int tileSize)
        {
            _tileResolution = tileResolution;
            _tileSize = tileSize;
            TileUriProvider = new NullTileUriProvider();
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
            if (_geoCoordinateCenter == null)
            {
                return;
            }

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
//
//                    var tileUri = string.Format("http://{0}.tile.opencyclemap.org/cycle/{1}/{2}/{3}.png", "a",
//                                                _levelOfDetail, tile.MapX, tile.MapY);
//                    using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
//                    using (var isoFileStream = isoStore.CreateFile(string.Format("opencycle_{0}_{1}_{2}.png", _levelOfDetail, tile.MapX, tile.MapY)))
//                    {
//                        var httpClient = new HttpClient();
//                        var stream = await httpClient.GetStreamAsync(new Uri(tileUri));
//                        stream.CopyTo(isoFileStream);
//                    }
//
//                    tile.Uri = string.Format("http://a.tile.opencyclemap.org/cycle/{zoom}/{x}/{y}.png", _levelOfDetail, tile.MapX, tile.MapY);

                    tile.Uri = TileUriProvider.GetTileUri(_levelOfDetail, tile.MapX, tile.MapY);
                }
            }

            _pixelCenter.X = pixelX;
            _pixelCenter.Y = pixelY;
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

        public Point GetOffsetInPixelsRelativeToCenter(GeoCoordinate geoCoordinate)
        {
            int pixelY;
            int pixelX;

            _mapMath.LatLongToPixelXY(geoCoordinate.Latitude, geoCoordinate.Longitude, _levelOfDetail, out pixelX, out pixelY);

            var offset = new Point(pixelX - _pixelCenter.X, pixelY - _pixelCenter.Y);
            return offset;
        }

        #endregion
    }
}