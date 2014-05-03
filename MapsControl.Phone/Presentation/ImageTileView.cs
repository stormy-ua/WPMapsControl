using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MapsControl.Presentation
{
    public class ImageTileView : MapEntityView, ITileView
    {
        #region Fields

        private readonly int _tileSize;

        #endregion

        #region Properties

        private BitmapImage Bitmap
        {
            get
            {
                return (BitmapImage)((Image)_visualElement).Source;
            }
        }

        public Uri Uri
        {
            get { return Bitmap.UriSource; }
            set
            {
                if (value == null)
                {
                    return;
                }

#if DESKTOP
                ((Image)_visualElement).Source = CreateImageSource(_tileSize);
#endif
                if (Bitmap.UriSource != value)
                {
#if DESKTOP
                    Bitmap.BeginInit();
#endif
                    Bitmap.UriSource = value;
#if DESKTOP
                    Bitmap.EndInit();
#endif
                }
            }
        }

        #endregion

        #region Constructors

        public ImageTileView(int tileSize)
            : base(CreateImage(tileSize))
        {
            _tileSize = tileSize;
        }

        #endregion

        #region Methods

        private static Image CreateImage(int tileSize)
        {
            var image = new Image
            {
                Width = tileSize,
                Height = tileSize,
                CacheMode = new BitmapCache(),
                RenderTransform = new TranslateTransform(),
#if WINDOWS_PHONE
                Source = CreateImageSource(tileSize)
#endif
            };
            return image;
        }

        private static ImageSource CreateImageSource(int tileSize)
        {
            var source = new BitmapImage
                {
                    DecodePixelHeight = tileSize,
                    DecodePixelWidth = tileSize,
#if WINDOWS_PHONE
                    DecodePixelType = DecodePixelType.Physical,
                    CreateOptions = BitmapCreateOptions.BackgroundCreation
#endif
                };
            return source;
        }

        #endregion
    }
}
