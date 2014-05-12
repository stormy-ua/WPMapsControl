using System.Device.Location;
using System.Windows;
using MapsControl.Phone.TileSources.MbTiles;
using MapsControl.TileUriProviders;
using Microsoft.Phone.Controls;

namespace SampleApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region GeoCoordinate

        public static readonly DependencyProperty GeoCoordinateProperty = DependencyProperty.Register(
            "GeoCoordinate", typeof (GeoCoordinate), typeof (MainPage), new PropertyMetadata(default(GeoCoordinate)));

        public GeoCoordinate GeoCoordinate
        {
            get { return (GeoCoordinate) GetValue(GeoCoordinateProperty); }
            set { SetValue(GeoCoordinateProperty, value); }
        }

        #endregion

        public MainPage()
        {
            InitializeComponent();

            //Map.TileUriProvider = TileUriProviders.OpenCycleMap
            //    .WithCache("OpenCycleCache/{zoom}/{x}_{y}.png");

            Map.TileUriProvider = new MbTilesSource("Kiev_Center_And_Left_Bank_13_17.db3");
            //Map.TileUriProvider = new MbTilesSource("Harkovskiy.db3");

            Loaded += MainPage_Loaded;
            GeoCoordinate = new GeoCoordinate(50.39346, 30.601);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var geoCoordinateWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default) {MovementThreshold = 1};
            geoCoordinateWatcher.PositionChanged +=
                (s, args) => Dispatcher.BeginInvoke(() => GeoCoordinate = args.Position.Location);

            geoCoordinateWatcher.Start(false);
        }

        private void ZoomInButtonOnClick(object sender, RoutedEventArgs e)
        {
            Map.LevelOfDetails++;
        }

        private void ZoomOutButtonOnClick(object sender, RoutedEventArgs e)
        {
            Map.LevelOfDetails--;
        }
    }
}