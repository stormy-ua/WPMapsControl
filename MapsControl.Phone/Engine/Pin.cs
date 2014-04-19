using System.Device.Location;

namespace MapsControl.Engine
{
    public class Pin : MapEntity
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
                    OnPropertyChanged();
                }
            }
        }

        #endregion
    }
}