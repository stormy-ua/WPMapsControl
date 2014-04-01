using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MapsControl.Engine;

namespace MapsControl.Rendering
{
    public class BorderTileElementBuilder /*: ITileElementBuilder*/
    {
        private readonly ITileController _tileController;

        public BorderTileElementBuilder(ITileController tileController)
        {
            _tileController = tileController;
        }

        public UIElement BuildTileElement(Tile tile)
        {
            var border = new Border();
            border.Width = _tileController.TileSize;
            border.Height = _tileController.TileSize;

            var grid = new Grid();
            border.Child = grid;

            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.BorderThickness = new Thickness(1);
            border.Background = new SolidColorBrush(Colors.Red);

            var textBlock = new TextBlock();
            textBlock.FontSize = 25;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.Text = string.Format("{0}, {1}", tile.MapX, tile.MapY);
            textBlock.Foreground = new SolidColorBrush(Colors.Black);
            grid.Children.Add(textBlock);
            return border;
        }
    }
}