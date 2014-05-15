using System.Device.Location;
using System.Diagnostics;
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

//            Map.TileUriProvider = TileUriProviders.OpenCycleMap
//                .WithCache("OpenCycleCache/{zoom}/{x}_{y}.png");

            Map.TileUriProvider = new MbTilesSource("Kiev_Center_And_Left_Bank_13_17.db3");
            //Map.TileUriProvider = new MbTilesSource("Harkovskiy.db3");
            //"C:\Program Files (x86)\Microsoft SDKs\Windows Phone\v8.0\Tools\IsolatedStorageExplorerTool\isetool.exe" rs de {8a1a21e6-d5e2-4f4b-944b-7189be4582f6} C:\temp\wp\IsolatedStore

            Loaded += MainPage_Loaded;
            GeoCoordinate = new GeoCoordinate(50.39346, 30.601);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var geoCoordinateWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default) {MovementThreshold = 1};
            geoCoordinateWatcher.PositionChanged +=
                (s, args) =>
                {
                    Debug.WriteLine("Position changed: {0}", args.Position);
                    GeoCoordinate = args.Position.Location;
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

        private void CenterCurrentLocation(object sender, RoutedEventArgs e)
        {
            Map.GeoCoordinateCenter = GeoCoordinate;
        }
    }
}