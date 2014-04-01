using System.Windows;
using MapsControl.Engine;

namespace MapsControl.Rendering
{
    public interface ITileElementBuilder
    {
        ITileElementPresenter BuildTileElement(Tile tile);
    }
}