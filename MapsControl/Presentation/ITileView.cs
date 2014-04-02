using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapsControl.Presentation
{
    public interface ITileView
    {
        UIElement VisualRoot { get; }

        double OffsetX { get; set; }

        double OffsetY { get; set; }

        Uri Uri { get; set; }
    }
}
