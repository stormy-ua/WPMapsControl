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
        public ObservableCollection<Pin> Pins { get; private set; } 

        public PolylineEntity(IEnumerable<GeoCoordinate> geoCoordinates)
        {
            Pins = new ObservableCollection<Pin>();

            if (geoCoordinates != null)
            {
                geoCoordinates.Select(c => new Pin { GeoCoordinate = c }).ForEach(Pins.Add);                
            }
        }
    }
}
