using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MapsControl.Engine;

namespace MapsControl.Rendering
{
    public class DebugImageTileElementBuilder /*: ITileElementBuilder*/
    {
        private readonly ITileController _tileController;

        public DebugImageTileElementBuilder(ITileController tileController)
        {
            _tileController = tileController;
        }

        public UIElement BuildTileElement(Tile tile)
        {
            var border = new Border();
            border.Width = _tileController.TileSize;
            border.Height = _tileController.TileSize;
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.BorderThickness = new Thickness(1);
            border.Background = new SolidColorBrush(Colors.Red);

            var grid = new Grid();
            border.Child = grid;

            var textBlock = new TextBlock();
            textBlock.FontSize = 25;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.Text = string.Format("{0}, {1}", tile.MapX, tile.MapY);
            textBlock.Foreground = new SolidColorBrush(Colors.Black);

            var image = new Image();
            grid.Children.Add(image);
            grid.Children.Add(textBlock);
            return border;
        }
    }
}