using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MapsControl.Engine;

namespace MapsControl.Presentation
{
    public class ImageTileView : MapEntityView, ITileView
    {
        #region Properties

        private BitmapImage Bitmap
        {
            get { return (BitmapImage)((Image)_visualElement).Source; }
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

                if (Bitmap.UriSource != value)
                {
                    Bitmap.UriSource = value;
                }
            }
        }

        #endregion

        #region Constructors

        public ImageTileView(int tileSize)
            : base(CreateImage(tileSize))
        {
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
                Source = new BitmapImage
                {
                    DecodePixelHeight = tileSize,
                    DecodePixelWidth = tileSize,
                    DecodePixelType = DecodePixelType.Physical,
                    CreateOptions = BitmapCreateOptions.BackgroundCreation
                },
                UseLayoutRounding = true,
                UseOptimizedManipulationRouting = true,
                RenderTransform = new TranslateTransform()
            };
            return image;
        }

        #endregion
    }
}
