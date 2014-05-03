using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapsControl.Presentation
{
    public interface IMapOverlayView : IMapEntityView
    {
        GeoCoordinate GeoCoordinate { get; set; }
        event EventHandler<GeoCoordinate> GeoCoordinateChanged;
    }
}
