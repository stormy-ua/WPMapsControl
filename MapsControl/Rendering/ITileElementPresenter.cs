using System.Windows;
using MapsControl.Engine;

namespace MapsControl.Rendering
{
    public interface ITileElementPresenter
    {
        UIElement VisualElement { get; }
        Tile Tile { get; }
    }
}