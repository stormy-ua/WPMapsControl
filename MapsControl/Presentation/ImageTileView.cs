using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MapsControl.Engine;

namespace MapsControl.Presentation
{
    public class ImageTileView : ITileView
    {
        #region Fields

        private readonly int _tileSize;
        private readonly Image _visualElement;

        #endregion

        #region Properties

        private TranslateTransform TranslateTransform
        {
            get { return (TranslateTransform)_visualElement.RenderTransform; }
        }

        private BitmapImage Bitmap
        {
            get { return (BitmapImage)_visualElement.Source; }
        }

        public UIElement VisualRoot
        {
            get { return _visualElement; }
        }

        public double OffsetX
        {
            get { return TranslateTransform.X; }
            set { TranslateTransform.X = value; }
        }

        public double OffsetY
        {
            get { return TranslateTransform.Y; }
            set { TranslateTransform.Y = value; }
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
        {
            _tileSize = tileSize;
            _visualElement = CreateImage();
        }

        #endregion

        #region Methods

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

        #endregion
    }
}
