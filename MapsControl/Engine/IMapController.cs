using System.Collections.Generic;
using System.Device.Location;
using System.Windows;
using MapsControl.Presentation;

namespace MapsControl.Engine
{
    public interface IMapController
    {
        IEnumerable<Tile> Tiles { get; }
        int TileSize { get; }
        int TileResolution { get; }
        GeoCoordinate GeoCoordinateCenter { get; }
        double LevelOfDetail { get; set; }
        ITileUriProvider TileUriProvider { get; set; }
        Size ViewWindowSize { get; set; }
        void SetGeoCoordinateCenter(GeoCoordinate geoCoordinate);
        void Move(Point2D offset);

        void AddPin(Pin pin);
        void PositionPin(Pin pin);

        void AddPolyline(PolylineEntity polylineEntity);
    }
}