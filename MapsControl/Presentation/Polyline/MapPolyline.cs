using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using MapsControl.Engine;
using MapsControl.Infrastructure;
using Microsoft.Phone.Maps.Controls;

namespace MapsControl.Presentation
{
    public class MapPolyline : MapElement, IPolylineEntityView
    {
        #region Fields

        private readonly Polyline _polyline = new Polyline
        {
            Stroke = new SolidColorBrush(Colors.Orange),
            StrokeThickness = 5
        };

        #endregion

        #region Path

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            "Path", typeof (GeoCoordinateCollection), typeof (MapPolyline),
            new PropertyMetadata(default(GeoCoordinateCollection), OnPathChanged));

        private static void OnPathChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var mapPolyline = (MapPolyline) dependencyObject;
            var geoCoordinates = (GeoCoordinateCollection) args.NewValue;

            mapPolyline.Content = mapPolyline._polyline;
            mapPolyline._polyline.Points.Clear();
            geoCoordinates.Select(c => new Point())
                .ForEach(mapPolyline._polyline.Points.Add);
            mapPolyline.RaiseGeoCoordinatesChanged();
        }

        [TypeConverter(typeof(GeoCoordinateCollectionConverter))]
        public GeoCoordinateCollection Path
        {
            get { return (GeoCoordinateCollection) GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        #endregion


        public FrameworkElement VisualRoot
        {
            get { return _polyline; }
        }

        public Point this[int index]
        {
            get { throw new NotImplementedException(); }
            set { _polyline.Points[index] = value; }
        }


        public event EventHandler GeoCoordinatesChanged;

        private void RaiseGeoCoordinatesChanged()
        {
            if (GeoCoordinatesChanged != null)
            {
                GeoCoordinatesChanged(this, EventArgs.Empty);
            }
        }
    }
}
