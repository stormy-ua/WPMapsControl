using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MapsControl.Engine;
using MapsControl.Rendering;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;

namespace MapsControl
{
/*    public interface ITileElement
    {
        UIElement UiElement { get; set; }
        void SetImage(BitmapImage bitmapImage);
    }

    public class ImageTileElement : ITileElement
    {
        public UIElement UiElement { get; set; }

        public void SetImage(BitmapImage bitmapImage)
        {
            var image = (Image)((Grid)(((Border)_tileElements[j]).Child)).Children[0];
            image.Source = new BitmapImage(new Uri(tile.Uri));
        }
    }*/

    public partial class MapsControl : UserControl
    {
        #region GeoCoordinateCenter Property

        public static readonly DependencyProperty GeoCoordinateCenterProperty =
            DependencyProperty.Register("GeoCoordinateCenter", typeof (GeoCoordinate), typeof (MapsControl),
                                        new PropertyMetadata(default(GeoCoordinate),
                                                             OnGeoCoordinateCenterPropertyChanged));

        [TypeConverter(typeof (GeoCoordinateConverter))]
        public GeoCoordinate GeoCoordinateCenter
        {
            get { return (GeoCoordinate) GetValue(GeoCoordinateCenterProperty); }
            set { SetValue(GeoCoordinateCenterProperty, value); }
        }

        #endregion

        public static readonly DependencyProperty LevelOfDetailsProperty =
            DependencyProperty.Register("LevelOfDetails", typeof (int), typeof (MapsControl), new PropertyMetadata(default(int), OnLevelOfDetailsPropertyChanged));

        public int LevelOfDetails
        {
            get { return (int) GetValue(LevelOfDetailsProperty); }
            set { SetValue(LevelOfDetailsProperty, value); }
        }

        private readonly ITileController _tileController;
        private readonly ITileElementBuilder _tileElementBuilder;
        private readonly IList<UIElement> _tileElements = new List<UIElement>();
        private readonly TranslateTransform _translateTransform = new TranslateTransform();
        private Size _windowSize = new Size();

        public MapsControl()
        {
            InitializeComponent();

            _tileController = new TileControllerBuilder().Build();
            _tileElementBuilder = new ImageTileElementBuilder(_tileController);

            foreach (var tile in _tileController.Tiles)
            {
                var tileElement = _tileElementBuilder.BuildTileElement(tile);

                _tileElements.Add(tileElement);
                tileElement.RenderTransform = _translateTransform;
                PART_Canvas.Children.Add(tileElement);
            }
        }

        private static void OnGeoCoordinateCenterPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var mapsControl = (MapsControl) dependencyObject;
            mapsControl.SetGeoCoordinateCenter((GeoCoordinate)args.NewValue);
        }

        private static void OnLevelOfDetailsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var mapsControl = (MapsControl)dependencyObject;
            mapsControl.SetLevelOfDetail((int)args.NewValue);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            _windowSize = arrangeBounds;
            PositionTileWindow();
            PART_Canvas.Clip = new RectangleGeometry
                {
                    Rect = new Rect(0, 0, arrangeBounds.Width, arrangeBounds.Height)
                };
            return base.ArrangeOverride(arrangeBounds);
        }

        private void PositionTileWindow()
        {
            int deltaX = (int)((_windowSize.Width / 2) - _tileController.TileWindowCenter.X);
            int deltaY = (int)((_windowSize.Height / 2) - _tileController.TileWindowCenter.Y);

            _translateTransform.X = deltaX;
            _translateTransform.Y = deltaY;
        }

        private void UpdateTiles()
        {
            for (int j = 0; j < _tileController.Tiles.Count(); ++j)
            {
                var tile = _tileController.Tiles.ElementAt(j);
                var image = (Image)_tileElements[j];
                image.Source = new BitmapImage(new Uri(tile.Uri));
            }
        }

        private void RedrawMap()
        {
            PositionTileWindow();
            UpdateTiles();
        }

        private void SetGeoCoordinateCenter(GeoCoordinate geoCoordinate)
        {
            _tileController.SetGeoCoordinateCenter(geoCoordinate);
            RedrawMap();
        }

        private void SetLevelOfDetail(int levelOfDetail)
        {
            _tileController.LevelOfDetail = levelOfDetail;
            RedrawMap();
        }

        private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            int dragOffsetX = (int)e.HorizontalChange;
            int dragOffsetY = (int)e.VerticalChange;

            _tileController.Move(dragOffsetX, dragOffsetY);
            RedrawMap();
        }

        private void OnDragStarted(object sender, DragStartedGestureEventArgs e)
        {
        }

        private void OnDragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
        }

        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
//            if (_tileController.LevelOfDetail > 17)
//            {
//                return;
//            }
//
//            _tileController.LevelOfDetail = (int)(_tileController.LevelOfDetail*(e.DistanceRatio/10));
//            RedrawMap();
        }
    }
}
