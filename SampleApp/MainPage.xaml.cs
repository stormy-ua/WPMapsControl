using System.Device.Location;
using System.Windows;
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

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
            Map.TileUriProvider = TileUriProviders.OpenCycleMap
                .WithCache("OpenCycleCache/{zoom}/{x}_{y}.png");

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

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
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