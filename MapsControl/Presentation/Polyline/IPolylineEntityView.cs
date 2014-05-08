using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MapsControl.Engine;
using Microsoft.Phone.Maps.Controls;

namespace MapsControl.Presentation
{
    public interface IPolylineEntityView
    {
        FrameworkElement VisualRoot { get; }

        GeoCoordinateCollection Path { get; }

        Point this[int index] { get; set; }

        event EventHandler GeoCoordinatesChanged;
    }
}
