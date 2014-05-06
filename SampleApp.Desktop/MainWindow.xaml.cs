using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MapsControl.Desktop.TileSources.MbTiles;
using MapsControl.TileUriProviders;

namespace SampleApp.Desktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

//            Map.TileUriProvider = TileUriProviders.GoogleHybridMap;
            Map.TileUriProvider = new MbTilesSource(@"C:\Maps\Kiev_Center_And_Left_Bank_13_17.db3");
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
