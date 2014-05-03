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
using MapsControl.Engine;
using MapsControl.Presentation;
using MapsControl.TileUriProviders;
#if WINDOWS_PHONE
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Reactive;
#endif
#if DESKTOP
using MapsControl.Desktop.Presentation.TypeConverters;
using System.Reactive.Linq;
#endif
using MapOverlay = MapsControl.Presentation.MapOverlay;
using MapPolyline = MapsControl.Presentation.MapPolyline;

namespace MapsControl
{
    [ContentProperty("MapElements")]
    public class MapsControl : Control, IMapView
    {
        #region Fields

        private readonly IMapPresenter _mapPresenter;
        private Panel _panel;
        private readonly MapCommands _mapCommands = new MapCommands();

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
            mapsControl._mapCommands.GeoCoordinateCentersSubject.OnNext((GeoCoordinate)args.NewValue);
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
            mapsControl._mapCommands.ZoomsSubject.OnNext((double)args.NewValue);
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

            mapsControl._mapCommands.TileUriProvidersSubject.OnNext(tileUriProvider);
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

#if DESKTOP
            _mapCommands.Translations = Observable.FromEventPattern<ManipulationDeltaEventArgs>(this, "ManipulationDelta")
                      .Select(args => args.EventArgs.DeltaManipulation.Translation.ToPoint2D());
            _mapCommands.SizeChanges = Observable.FromEventPattern<SizeChangedEventArgs>(this, "SizeChanged")
                      .Select(args => args.EventArgs.NewSize);
            _mapCommands.Initialized = Observable.FromEventPattern<RoutedEventArgs>(this, "Loaded")
                      .Select(args => args.EventArgs);

            _mapCommands.SizeChanges.Subscribe(size => _panel.Clip = new RectangleGeometry { Rect = new Rect(0, 0, size.Width, size.Height) });
            _mapCommands.EntityViewAdded = Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(MapElements, "CollectionChanged")
                                        .Where(args => args.EventArgs.Action == NotifyCollectionChangedAction.Add)
                                        .Select(args => args.EventArgs.NewItems.OfType<IMapEntityView>())
                                        .Where(entityView => entityView != null && entityView.Any());

            var mouseDownPoints = Observable.FromEventPattern<MouseButtonEventArgs>(this, "MouseLeftButtonDown");
            var mouseUpPoints = Observable.FromEventPattern<MouseButtonEventArgs>(this, "MouseLeftButtonUp");
            var mouseMovePoints = Observable.FromEventPattern<MouseEventArgs>(this, "MouseMove")
                .Select(e => e.EventArgs.GetPosition(null))
                .SkipUntil(mouseDownPoints)
                .TakeUntil(mouseUpPoints);
            _mapCommands.Translations = mouseMovePoints
                .Skip(1)
                .CombineLatest(mouseMovePoints, (point2, point1) => (point2 - point1).ToPoint2D())
                .Repeat();
#endif

#if WINDOWS_PHONE
            _mapCommands.Translations = Observable.FromEvent<ManipulationDeltaEventArgs>(this, "ManipulationDelta")
                      .Select(args => args.EventArgs.DeltaManipulation.Translation.ToPoint2D());
            _mapCommands.SizeChanges = Observable.FromEvent<SizeChangedEventArgs>(this, "SizeChanged")
                      .Select(args => args.EventArgs.NewSize);
            _mapCommands.Initialized = Observable.FromEvent<RoutedEventArgs>(this, "Loaded")
                      .Select(args => args.EventArgs);

            _mapCommands.SizeChanges.Subscribe(size => _panel.Clip = new RectangleGeometry { Rect = new Rect(0, 0, size.Width, size.Height) });
            _mapCommands.EntityViewAdded = Observable.FromEvent<NotifyCollectionChangedEventArgs>(MapElements, "CollectionChanged")
                                        .Where(args => args.EventArgs.Action == NotifyCollectionChangedAction.Add)
                                        .Select(args => args.EventArgs.NewItems.OfType<IMapEntityView>())
                                        .Where(entityView => entityView != null && entityView.Any());
#endif

            _mapPresenter = new MapPresenter(this, _mapCommands, 5);
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _panel = (Panel)GetTemplateChild("PART_Panel");
        }

        #region IMapView

        public void Add(IMapEntityView mapEntityView)
        {
            _panel.Children.Add(mapEntityView.VisualRoot);
        }

        public void Remove(IMapEntityView mapEntityView)
        {
            _panel.Children.Remove(mapEntityView.VisualRoot);
        }

        #endregion
    }
}
