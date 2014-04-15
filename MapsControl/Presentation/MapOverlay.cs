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
using Microsoft.Phone.Maps.Controls;

namespace MapsControl.Presentation
{
    [ContentProperty("Content")]
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

        #region ContentProperty

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof (FrameworkElement), typeof (MapOverlay), new PropertyMetadata(default(UIElement)));

        public FrameworkElement Content
        {
            get { return (FrameworkElement) GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        #endregion

        #region Properties

        private TranslateTransform TranslateTransform
        {
            get { return (TranslateTransform)Content.RenderTransform; }
        }

        public FrameworkElement VisualRoot
        {
            get { return Content; }
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

        #endregion

        #region Events

        public event EventHandler<GeoCoordinate> GeoCoordinateChanged;

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
