using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;
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
#if WINDOWS_PHONE
using Microsoft.Phone.Maps.Controls;
#endif
#if DESKTOP
using MapsControl.Desktop.Presentation.TypeConverters;
#endif

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
            "Path", typeof(ObservableCollection<GeoCoordinate>), typeof(MapPolyline),
            new PropertyMetadata(default(ObservableCollection<GeoCoordinate>), OnPathChanged));

        private static void OnPathChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var mapPolyline = (MapPolyline) dependencyObject;
            var geoCoordinates = (ObservableCollection<GeoCoordinate>)args.NewValue;

            mapPolyline.Content = mapPolyline._polyline;
            mapPolyline._polyline.Points.Clear();
            geoCoordinates.Select(c => new Point())
                .ForEach(mapPolyline._polyline.Points.Add);
            mapPolyline.RaiseGeoCoordinatesChanged();
        }

        [TypeConverter(typeof(GeoCoordinateCollectionConverter))]
        public ObservableCollection<GeoCoordinate> Path
        {
            get { return (ObservableCollection<GeoCoordinate>)GetValue(PathProperty); }
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

    public interface IMapLineEntityView : IMapEntityView
    {
        GeoCoordinate Begin { get; set; }
        GeoCoordinate End { get; set; }
        
        Point EndOffset { get; set; }

        event EventHandler<GeoCoordinate> BeginChanged;
        event EventHandler<GeoCoordinate> EndChanged;
    }

    public class MapLine : MapElement, IMapLineEntityView
    {
        #region Fields

        private readonly Polyline _line = new Polyline
        {
            Stroke = new SolidColorBrush(Colors.Orange),
            StrokeThickness = 5
        };

        #endregion

        #region Path

        public static readonly DependencyProperty BeginProperty =
            DependencyProperty.Register("Begin", typeof (GeoCoordinate), typeof (MapLine), new PropertyMetadata(default(GeoCoordinate), OnBeginChanged));

        private static void OnBeginChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var mapLine = (MapLine)dependencyObject;
            var begin = (GeoCoordinate)args.NewValue;
            mapLine.OnBeginChanged(begin);
        }

        [TypeConverter(typeof(GeoCoordinateConverter))]
        public GeoCoordinate Begin
        {
            get { return (GeoCoordinate) GetValue(BeginProperty); }
            set { SetValue(BeginProperty, value); }
        }

        public static readonly DependencyProperty EndProperty =
            DependencyProperty.Register("End", typeof (GeoCoordinate), typeof (MapLine), new PropertyMetadata(default(GeoCoordinate), OnEndChanged));

        private static void OnEndChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var mapLine = (MapLine)dependencyObject;
            var end = (GeoCoordinate)args.NewValue;
            mapLine.OnEndChanged(end);
        }

        [TypeConverter(typeof(GeoCoordinateConverter))]
        public GeoCoordinate End
        {
            get { return (GeoCoordinate) GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        #endregion

        #region Constructors

        public MapLine()
        {
            //DefaultStyleKey = typeof(MapOverlay);
            RenderTransform = new TranslateTransform();

            _line.Points.Add(new Point());
            _line.Points.Add(new Point());
        }

        #endregion

        public event EventHandler<GeoCoordinate> BeginChanged;

        public event EventHandler<GeoCoordinate> EndChanged;

        private TranslateTransform TranslateTransform
        {
            get { return (TranslateTransform)RenderTransform; }
        }
        
        public FrameworkElement VisualRoot
        {
            get { return _line; }
        }

//        public Point Offset
//        {
//            get { return new Point(TranslateTransform.X, TranslateTransform.Y); }
//            set
//            {
//                TranslateTransform.X = value.X;
//                TranslateTransform.Y = value.Y;
//            }
//        }

        public Point Offset
        {
            get { return _line.Points[0]; }
            set { _line.Points[0] = value; }
        }

        public Point EndOffset
        {
            get { return _line.Points[1]; }
            set { _line.Points[1] = value; }
        }

        private void OnBeginChanged(GeoCoordinate geoCoordinate)
        {
            if (BeginChanged != null)
            {
                BeginChanged(this, geoCoordinate);
            }
        }

        private void OnEndChanged(GeoCoordinate geoCoordinate)
        {
            if (EndChanged != null)
            {
                EndChanged(this, geoCoordinate);
            }
        }
    }
}
