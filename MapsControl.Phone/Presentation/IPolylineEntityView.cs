using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MapsControl.Engine;

namespace MapsControl.Presentation
{
    public interface IPolylineEntityView
    {
        FrameworkElement VisualRoot { get; }

        ObservableCollection<GeoCoordinate> Path { get; }

        Point this[int index] { get; set; }

        event EventHandler GeoCoordinatesChanged;
    }
}
