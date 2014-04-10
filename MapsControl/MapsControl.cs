using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MapsControl.Engine;
using MapsControl.Presentation;
using MapsControl.TileUriProviders;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;

namespace MapsControl
{
    [ContentProperty("MapElements")]
    public class MapsControl : Control
    {
        #region Fields

        private readonly IMapController _mapController;
        private readonly IList<TilePresenter> _tileElements = new List<TilePresenter>();
        private readonly IList<MapEntityPresenter> _mapEntityPresenters = 
            new List<MapEntityPresenter>();
        private Panel _canvas;
        private Size _windowSize;

        #endregion

        #region GeoCoordinateCenter Property

        public static readonly DependencyProperty GeoCoordinateCenterProperty =
            DependencyProperty.Register("GeoCoordinateCenter", typeof(GeoCoordinate), typeof(MapsControl),
                                        new PropertyMetadata(default(GeoCoordinate),
                                                             OnGeoCoordinateCenterPropertyChanged));

        [TypeConverter(typeof(GeoCoordinateConverter))]
        public GeoCoordinate GeoCoordinateCenter
        {
            get { return (GeoCoordinate)GetValue(GeoCoordinateCenterProperty); }
            set { SetValue(GeoCoordinateCenterProperty, value); }
        }

        private static void OnGeoCoordinateCenterPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var mapsControl = (MapsControl)dependencyObject;
            mapsControl.SetGeoCoordinateCenter((GeoCoordinate)args.NewValue);
        }

        #endregion

        #region LevelOfDetails Property

        public static readonly DependencyProperty LevelOfDetailsProperty =
            DependencyProperty.Register("LevelOfDetails", typeof (double), typeof (MapsControl),
                                        new PropertyMetadata(default(double), OnLevelOfDetailsPropertyChanged));

        public double LevelOfDetails
        {
            get { return (double) GetValue(LevelOfDetailsProperty); }
            set { SetValue(LevelOfDetailsProperty, value); }
        }

        private static void OnLevelOfDetailsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var mapsControl = (MapsControl)dependencyObject;
            mapsControl.SetLevelOfDetail((double)args.NewValue);
        }

        #endregion

        #region GeoPosition Attached Property

        public static readonly DependencyProperty GeoPositionProperty =
            DependencyProperty.RegisterAttached("GeoPosition", typeof (GeoCoordinate), typeof (MapsControl),
                                                new PropertyMetadata(default(GeoCoordinate), OnGeoPositionPropertyChanged));

        public static void SetGeoPosition(UIElement element, GeoCoordinate value)
        {
            element.SetValue(GeoPositionProperty, value);
        }

        [TypeConverter(typeof(GeoCoordinateConverter))]
        public static GeoCoordinate GetGeoPosition(UIElement element)
        {
            return (GeoCoordinate) element.GetValue(GeoPositionProperty);
        }

        private static void OnGeoPositionPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var element = (FrameworkElement)dependencyObject;
            var geoCoordinate = (GeoCoordinate)args.NewValue;
            var mapsControl = GetMap(element);

            if (mapsControl == null)
            {
                return;
            }

            mapsControl.PositionMapElement(element, geoCoordinate);
        }

        #endregion

        #region Map Attached Property

        public static readonly DependencyProperty MapProperty =
            DependencyProperty.RegisterAttached("Map", typeof (MapsControl), typeof (MapsControl),
                                                new PropertyMetadata(default(MapsControl)));

        public static void SetMap(UIElement element, MapsControl value)
        {
            element.SetValue(MapProperty, value);
        }

        public static MapsControl GetMap(UIElement element)
        {
            return (MapsControl) element.GetValue(MapProperty);
        }

        #endregion

        #region TileUriProvider Property

        public static readonly DependencyProperty TileUriProviderProperty =
            DependencyProperty.Register("TileUriProvider", typeof (ITileUriProvider), typeof (MapsControl),
            new PropertyMetadata(new NullTileUriProvider(), OnTileUriProviderPropertyChanged));

        public ITileUriProvider TileUriProvider
        {
            get { return (ITileUriProvider) GetValue(TileUriProviderProperty); }
            set { SetValue(TileUriProviderProperty, value); }
        }

        private static void OnTileUriProviderPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var tileUriProvider = (ITileUriProvider)args.NewValue;
            var mapsControl = dependencyObject as MapsControl;

            if (mapsControl == null)
            {
                return;
            }

            mapsControl.SetTileUriProvider(tileUriProvider);
        }

        #endregion

        #region Properties

        public ObservableCollection<FrameworkElement> MapElements { get; private set; }

        #endregion

        #region Constructors

        public MapsControl()
        {
            DefaultStyleKey = typeof (MapsControl);
            MapElements = new ObservableCollection<FrameworkElement>();
            MapElements.CollectionChanged += (sender, args) =>
                {
                    if (args.Action != NotifyCollectionChangedAction.Add)
                    {
                        return;
                    }

                    foreach (var mapElement in args.NewItems.OfType<FrameworkElement>())
                    {
                        SetMap(mapElement, this);
                    }
                };

            _mapController = new MapController(5, 256);
            ManipulationDelta += OnManipulationDelta;
            Loaded += OnLoaded;
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = (Panel)GetTemplateChild("PART_Panel");
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            _windowSize = arrangeBounds;
            _mapController.ViewWindowSize = _windowSize;

            _canvas.Clip = new RectangleGeometry
            {
                Rect = new Rect(0, 0, _windowSize.Width, _windowSize.Height)
            };
            return base.ArrangeOverride(arrangeBounds);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            foreach (var tile in _mapController.Tiles)
            {
                var tileView = new ImageTileView(_mapController.TileSize);
                var tileElement = new TilePresenter(tileView, tile);

                _tileElements.Add(tileElement);
                _canvas.Children.Add(tileView.VisualRoot);
            }

            foreach (var mapElement in MapElements)
            {
                var pin = new Pin { GeoCoordinate = GetGeoPosition(mapElement) };
                var elementView = new FrameworkElementView(mapElement);
                var mapEntityPresenter = new PinEntityPresenter(elementView, pin);
                
                _mapController.AddPin(pin);
                _mapEntityPresenters.Add(mapEntityPresenter);
                _canvas.Children.Add(elementView.VisualRoot);
            }
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs args)
        {
            _mapController.Move(args.DeltaManipulation.Translation.ToPoint2D());
        }

        private void SetGeoCoordinateCenter(GeoCoordinate geoCoordinate)
        {
            _mapController.SetGeoCoordinateCenter(geoCoordinate);
        }

        private void SetLevelOfDetail(double levelOfDetail)
        {
            _mapController.LevelOfDetail = levelOfDetail;
        }

        private void PositionMapElement(FrameworkElement element, GeoCoordinate geoCoordinate)
        {
            var mapEntityPresenter = _mapEntityPresenters.FirstOrDefault(e => e.View.VisualRoot == element);

            if (mapEntityPresenter != null && mapEntityPresenter.MapEntity is Pin)
            {
                var pin = (Pin) mapEntityPresenter.MapEntity;
                pin.GeoCoordinate = geoCoordinate;
                _mapController.PositionPin(pin);
            }
        }

        private void SetTileUriProvider(ITileUriProvider tileUriProvider)
        {
            _mapController.TileUriProvider = tileUriProvider;
        }
    }
}
