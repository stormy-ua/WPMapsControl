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
        GeoCoordinate GeoCoordinateCenter { get; }
        double LevelOfDetail { get; set; }
        ITileUriProvider TileUriProvider { get; set; }
        Size ViewWindowSize { get; set; }
        void SetGeoCoordinateCenter(GeoCoordinate geoCoordinate);
        void Move(int deltaPixelX, int deltaPixelY);
        Point GetOffsetInPixelsRelativeToCenter(GeoCoordinate geoCoordinate);
    }
}