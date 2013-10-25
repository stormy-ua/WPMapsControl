using System.Collections.Generic;
using System.Device.Location;
using System.Windows;

namespace MapsControl.Engine
{
    public interface ITileController
    {
        IEnumerable<Tile> Tiles { get; }
        int TileSize { get; }
        int TileResolution { get; }
        Point TileWindowCenter { get; }
        GeoCoordinate GeoCoordinateCenter { get; }
        int LevelOfDetail { get; set; }
        void SetGeoCoordinateCenter(GeoCoordinate geoCoordinate);
        void Move(int deltaPixelX, int deltaPixelY);
    }
}