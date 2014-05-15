using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MapsControl.Engine;

namespace MapsControl.Presentation
{
    public class ImageTileView : MapEntityView, ITileView
    {
        #region Fields

        private readonly int _tileSize;
        private TileSource _tileSource;

        #endregion

        #region Properties

        private BitmapImage Bitmap
        {
            get
            {
                return (BitmapImage)((Image)_visualElement).Source;
            }
        }

        public TileSource TileSource
        {
            get { return _tileSource; }
            set
            {
                if (_tileSource == value)
                {
                    return;
                }

                _tileSource = value;

                try
                {
                    if (_tileSource == TileSource.Empty)
                    {
                        //TODO: reset image
                        if (Bitmap != null)
                        {
                            Bitmap.UriSource = null;
                        }
                    }
                    if (_tileSource is TileUriSource)
                    {
#if DESKTOP
                        ((Image)_visualElement).Source = CreateImageSource(_tileSize);
                        Bitmap.BeginInit();
#endif

                        Bitmap.UriSource = ((TileUriSource)_tileSource).Uri;

#if DESKTOP
                        Bitmap.EndInit();
#endif
                    }
                    else if (_tileSource is TileByteArraySource)
                    {
#if DESKTOP

                        ((Image)_visualElement).Source = ToBitmapImage(((TileByteArraySource)_tileSource).TileBytes);
#endif
#if WINDOWS_PHONE
                        using (var stream = new MemoryStream(((TileByteArraySource)_tileSource).TileBytes))
                        {
                            Bitmap.SetSource(stream);
                        }
#endif
                    }
                    else
                    {
#if DESKTOP
                        ((Image)_visualElement).Source = CreateImageSource(_tileSize);
#endif
                    }
                }
                catch (Exception)
                {
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

#if DESKTOP
        private static BitmapSource ToBitmapImage(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                return decoder.Frames[0];
            }
        }
#endif

        private static Image CreateImage(int tileSize)
        {
            var image = new Image
            {
                Width = tileSize,
                Height = tileSize,
                CacheMode = new BitmapCache(),
                RenderTransform = new TranslateTransform(),
                Source = CreateImageSource(tileSize)
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
