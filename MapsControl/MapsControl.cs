using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
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
using MapsControl.Rendering;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;

namespace MapsControl
{
    [ContentProperty("MapElements")]
    public class MapsControl : Control
    {
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

        #endregion

        public static readonly DependencyProperty LevelOfDetailsProperty =
            DependencyProperty.Register("LevelOfDetails", typeof(int), typeof(MapsControl), new PropertyMetadata(default(int), OnLevelOfDetailsPropertyChanged));

        public int LevelOfDetails
        {
            get { return (int)GetValue(LevelOfDetailsProperty); }
            set { SetValue(LevelOfDetailsProperty, value); }
        }

        private readonly ITileController _tileController;
        private readonly ITileElementBuilder _tileElementBuilder;
        private readonly IList<UIElement> _tileElements = new List<UIElement>();
        private readonly TranslateTransform _translateTransform = new TranslateTransform();
        private Canvas _canvas;
        private Size _windowSize = new Size();

        public MapsControl()
        {
            DefaultStyleKey = typeof(MapsControl);
            MapElements = new List<UIElement>();

            _tileController = new TileControllerBuilder().Build();
            _tileElementBuilder = new ImageTileElementBuilder(_tileController);

            ManipulationDelta += OnManipulationDelta;
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs args)
        {
            int dragOffsetX = (int)args.DeltaManipulation.Translation.X;
            int dragOffsetY = (int)args.DeltaManipulation.Translation.Y;

            _tileController.Move(dragOffsetX, dragOffsetY);
            RedrawMap();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = (Canvas)GetTemplateChild("PART_Canvas");

            BuildTiles();
            RedrawMap();
        }

        private static void OnGeoCoordinateCenterPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var mapsControl = (MapsControl)dependencyObject;
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
            _canvas.Clip = new RectangleGeometry
            {
                Rect = new Rect(0, 0, arrangeBounds.Width, arrangeBounds.Height)
            };
            return base.ArrangeOverride(arrangeBounds);
        }

        public List<UIElement> MapElements { get; private set; }

        private void BuildTiles()
        {
            foreach (var tile in _tileController.Tiles)
            {
                var tileElement = _tileElementBuilder.BuildTileElement(tile);

                _tileElements.Add(tileElement);
                tileElement.RenderTransform = _translateTransform;
                _canvas.Children.Add(tileElement);
            }
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
            if (!_tileElements.Any())
            {
                return;
            }

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
    }
}
