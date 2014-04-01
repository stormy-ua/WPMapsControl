using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MapsControl.Engine;

namespace MapsControl.Rendering
{
    public class ImageTileElementPresenter : TileElementPresenter
    {
        private readonly int _tileSize;

        public ImageTileElementPresenter(int tileSize, Tile tile)
            : base(tile)
        {
            _tileSize = tileSize;
            VisualElement = CreateImage();

            OnTileOffsetChanged();
            OnTileUriChanged();
        }

        private Image CreateImage()
        {
            var image = new Image
            {
                Width = _tileSize,
                Height = _tileSize,
                CacheMode = new BitmapCache(),
                Source = new BitmapImage
                {
                    DecodePixelHeight = _tileSize,
                    DecodePixelWidth = _tileSize,
                    DecodePixelType = DecodePixelType.Physical,
                    CreateOptions = BitmapCreateOptions.BackgroundCreation
                },
                UseLayoutRounding = true,
                UseOptimizedManipulationRouting = true,
                RenderTransform = new TranslateTransform()
            };
            return image;
        }

        protected sealed override void OnTileUriChanged()
        {
            if (Tile.Uri == null)
            {
                return;
            }

            var image = (Image)VisualElement;
            var bitmapImage = image.Source as BitmapImage;
            if (bitmapImage.UriSource != Tile.Uri)
            {
                bitmapImage.UriSource = Tile.Uri;
            }
        }

        protected sealed override void OnTileOffsetChanged()   
        {
            var translateTransform = (TranslateTransform)VisualElement.RenderTransform;
            translateTransform.X = Tile.OffsetX;
            translateTransform.Y = Tile.OffsetY;
        }
    }
}