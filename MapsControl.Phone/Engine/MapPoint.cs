using System.Device.Location;

namespace MapsControl.Engine
{
    public class MapPoint : MapEntity
    {
        #region Fields

        private GeoCoordinate _geoCoordinate;

        #endregion

        #region Properties

        public GeoCoordinate GeoCoordinate
        {
            get { return _geoCoordinate; }
            set
            {
                if (_geoCoordinate != value)
                {
                    _geoCoordinate = value;
                }
            }
        }

        #endregion
    }

    public class MapLineEntity
    {
        public MapPoint Begin { get; private set; }
        public MapPoint End { get; private set; }

        public MapLineEntity(MapPoint begin, MapPoint end)
        {
            Begin = begin;
            End = end;
        }
    }
}