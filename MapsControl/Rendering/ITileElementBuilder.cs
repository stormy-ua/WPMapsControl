using System.Windows;
using MapsControl.Engine;

namespace MapsControl.Rendering
{
    public interface ITileElementBuilder
    {
        UIElement BuildTileElement(Tile tile);
    }
}