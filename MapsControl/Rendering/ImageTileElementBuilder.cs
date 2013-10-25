using System.Windows;
using System.Windows.Controls;
using MapsControl.Engine;

namespace MapsControl.Rendering
{
    public class ImageTileElementBuilder : ITileElementBuilder
    {
        private readonly ITileController _tileController;

        public ImageTileElementBuilder(ITileController tileController)
        {
            _tileController = tileController;
        }

        public UIElement BuildTileElement(Tile tile)
        {
            var image = new Image
                {
                    Width = _tileController.TileSize,
                    Height = _tileController.TileSize
                };
            Canvas.SetLeft(image, tile.X * _tileController.TileSize);
            Canvas.SetTop(image, tile.Y * _tileController.TileSize);
            return image;
        }
    }
}