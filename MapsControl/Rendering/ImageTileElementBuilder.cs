using System.Windows;
using System.Windows.Controls;
using MapsControl.Engine;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MapsControl.Rendering
{
    public class ImageTileElementBuilder /*: ITileElementBuilder*/
    {
        private readonly ITileController _tileController;

        public ImageTileElementBuilder(ITileController tileController)
        {
            _tileController = tileController;
        }

        public Image BuildTileElement(Tile tile)
        {
            var image = new Image
                {
                    Width = _tileController.TileSize,
                    Height = _tileController.TileSize,
                    CacheMode = new BitmapCache(),
                    Source = new BitmapImage 
                    { 
                        DecodePixelHeight = _tileController.TileSize, 
                        DecodePixelWidth = _tileController.TileSize, 
                        DecodePixelType = DecodePixelType.Physical, 
                        CreateOptions = BitmapCreateOptions.BackgroundCreation
                    },
                    UseLayoutRounding = true,
                    UseOptimizedManipulationRouting = true,

                };
            return image;
        }
    }
}