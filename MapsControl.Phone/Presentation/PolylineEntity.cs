using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapsControl.Engine;
using MapsControl.Infrastructure;

namespace MapsControl.Presentation
{
    public class PolylineEntity
    {
        public ObservableCollection<MapPoint> Pins { get; private set; } 

        public PolylineEntity(IEnumerable<GeoCoordinate> geoCoordinates)
        {
            Pins = new ObservableCollection<MapPoint>();

            if (geoCoordinates != null)
            {
                geoCoordinates.Select(c => new MapPoint { GeoCoordinate = c }).ForEach(Pins.Add);                
            }
        }
    }
}
