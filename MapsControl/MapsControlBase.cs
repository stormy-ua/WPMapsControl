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
    public class MapsControlBase : Control, IMapView
    {
        #region Fields

        protected MapPresenter _mapPresenter;
        protected Panel _panel;
        protected readonly MapCommands _mapCommands = new MapCommands();

        #endregion

        #region GeoCoordinateCenter Property

        public static readonly DependencyProperty GeoCoordinateCenterProperty =
            DependencyProperty.Register("GeoCoordinateCenter", typeof(GeoCoordinate), typeof(MapsControlBase),
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
            var mapsControl = (MapsControlBase)dependencyObject;
            mapsControl._mapCommands.GeoCoordinateCentersSubject.OnNext((GeoCoordinate)args.NewValue);
        }

        #endregion

        #region LevelOfDetails Property

        public static readonly DependencyProperty LevelOfDetailsProperty =
            DependencyProperty.Register("LevelOfDetails", typeof(double), typeof(MapsControlBase),
                                        new PropertyMetadata(default(double), OnLevelOfDetailsPropertyChanged));

        public double LevelOfDetails
        {
            get { return (double) GetValue(LevelOfDetailsProperty); }
            set { SetValue(LevelOfDetailsProperty, value); }
        }

        private static void OnLevelOfDetailsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var mapsControl = (MapsControlBase)dependencyObject;
            mapsControl._mapCommands.ZoomsSubject.OnNext((double)args.NewValue);
        }

        #endregion

        #region TileUriProvider Property

        public static readonly DependencyProperty TileUriProviderProperty =
            DependencyProperty.Register("TileUriProvider", typeof(ITileSourceProvider), typeof(MapsControlBase),
            new PropertyMetadata(new NullTileUriProvider(), OnTileUriProviderPropertyChanged));

        public ITileSourceProvider TileUriProvider
        {
            get { return (ITileSourceProvider) GetValue(TileUriProviderProperty); }
            set { SetValue(TileUriProviderProperty, value); }
        }

        private static void OnTileUriProviderPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var tileUriProvider = (ITileSourceProvider)args.NewValue;
            var mapsControl = dependencyObject as MapsControlBase;

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

        public MapsControlBase()
        {
            MapElements = new ObservableCollection<MapElement>();
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _panel = (Panel)GetTemplateChild("PART_Panel");
        }

        #region IMapView

        public void Add(IMapEntityView mapEntityView, XMapLayer layer)
        {
            _panel.Children.Add(mapEntityView.VisualRoot);
            //Panel.SetZIndex(mapEntityView.VisualRoot, (int)layer);
        }

        public void Remove(IMapEntityView mapEntityView)
        {
            _panel.Children.Remove(mapEntityView.VisualRoot);
        }

        #endregion
    }
}
