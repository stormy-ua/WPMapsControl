using System.Windows;

namespace MapsControl.Presentation
{
    public interface IMapEntityView
    {
        FrameworkElement VisualRoot { get; }
        Point Offset { get; set; }
    }
}