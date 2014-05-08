using System.Device.Location;
using System.Windows;
using MapsControl.Phone.TileSources.MbTiles;
using MapsControl.TileUriProviders;
using Microsoft.Phone.Controls;

namespace SampleApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            Map.TileUriProvider = TileUriProviders.OpenCycleMap
                .WithCache("OpenCycleCache/{zoom}/{x}_{y}.png");

            //Map.TileUriProvider = new MbTilesSource("Kiev_Center_And_Left_Bank_13_17.db3");

            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var geoCoordinateWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            geoCoordinateWatcher.MovementThreshold = 1;
            geoCoordinateWatcher.PositionChanged +=
                (s, args) =>
                {
                    if (Marker == null)
                    {
                        return;
                    }

                    Dispatcher.BeginInvoke(() => Marker.GeoCoordinate = args.Position.Location);
                };

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