using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
#if WINDOWS_PHONE
using Microsoft.Phone.Maps.Controls;
#endif
#if DESKTOP
using MapsControl.Desktop.Presentation.TypeConverters;
#endif

namespace MapsControl.Presentation
{
    public class MapOverlay : MapElement, IMapOverlayView
    {
        #region GeoCoordinate Property

        public static readonly DependencyProperty GeoCoordinateProperty =
            DependencyProperty.Register("GeoCoordinate", typeof(GeoCoordinate), typeof(MapOverlay),
                                        new PropertyMetadata(default(GeoCoordinate),
                                                             OnGeoCoordinatePropertyChanged));

        [TypeConverter(typeof(GeoCoordinateConverter))]
        public GeoCoordinate GeoCoordinate
        {
            get { return (GeoCoordinate)GetValue(GeoCoordinateProperty); }
            set { SetValue(GeoCoordinateProperty, value); }
        }

        private static void OnGeoCoordinatePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var mapOverlay = (MapOverlay) dependencyObject;
            var geoCoordinate = args.NewValue as GeoCoordinate;
            mapOverlay.OnGeoCoordinateChanged(geoCoordinate);
        }

        #endregion

        #region Properties

        private TranslateTransform TranslateTransform
        {
            get { return (TranslateTransform)RenderTransform; }
        }

        public FrameworkElement VisualRoot
        {
            get { return this; }
        }

        public Point Offset
        {
            get { return new Point(TranslateTransform.X, TranslateTransform.Y); }
            set
            {
                TranslateTransform.X = value.X;
                TranslateTransform.Y = value.Y;
            }
        }

        #endregion

        #region Events

        public event EventHandler<GeoCoordinate> GeoCoordinateChanged;

        #endregion

        #region Constructors
            
        public MapOverlay()
        {
            DefaultStyleKey = typeof(MapOverlay);
            RenderTransform = new TranslateTransform();
        }

        #endregion

        #region Methods

        private void OnGeoCoordinateChanged(GeoCoordinate geoCoordinate)
        {
            if (GeoCoordinateChanged != null)
            {
                GeoCoordinateChanged(this, geoCoordinate);
            }
        }

        #endregion
    }
}
