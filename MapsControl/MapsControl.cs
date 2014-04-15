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
using MapOverlay = MapsControl.Presentation.MapOverlay;
using MapPolyline = MapsControl.Presentation.MapPolyline;

namespace MapsControl
{
    [ContentProperty("MapElements")]
    public class MapsControl : Control
    {
        #region Fields

        private readonly IMapController _mapController = new MapController(5, 256);
        private readonly IList<TilePresenter> _tileElements = new List<TilePresenter>();
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
            mapsControl._mapController.SetGeoCoordinateCenter((GeoCoordinate)args.NewValue);
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
            mapsControl._mapController.LevelOfDetail = (double)args.NewValue;
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

            mapsControl._mapController.TileUriProvider = tileUriProvider;
        }

        #endregion

        #region Properties

        public ObservableCollection<MapElement> MapElements { get; private set; }

        #endregion

        #region Constructors

        public MapsControl()
        {
            DefaultStyleKey = typeof (MapsControl);
            MapElements = new ObservableCollection<MapElement>();

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

            foreach (var mapOverlay in MapElements.OfType<MapOverlay>())
            {
                var pin = new Pin { GeoCoordinate = mapOverlay.GeoCoordinate };
                var mapEntityPresenter = new MapOverlayPresenter(_mapController, mapOverlay, pin);

                _mapController.AddPin(pin);
                _canvas.Children.Add(mapOverlay.Content);
            }

            foreach (var mapPolyline in MapElements.OfType<MapPolyline>())
            {
                var polylineEntity = new PolylineEntity(mapPolyline.Path);
                var polylineEntityPresenter = new PolylineEntityPresenter(_mapController, mapPolyline, polylineEntity);

                _mapController.AddPolyline(polylineEntity);
                _canvas.Children.Add(mapPolyline.VisualRoot);
            }
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs args)
        {
            _mapController.Move(args.DeltaManipulation.Translation.ToPoint2D());
        }
    }
}
