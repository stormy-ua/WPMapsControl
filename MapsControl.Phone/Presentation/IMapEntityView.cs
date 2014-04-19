using System.Windows;

namespace MapsControl.Presentation
{
    public interface IMapEntityView
    {
        FrameworkElement VisualRoot { get; }
        double OffsetX { get; set; }
        double OffsetY { get; set; }
    }
}