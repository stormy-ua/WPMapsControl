using System;
using System.Device.Location;
using System.Windows;
using System.Windows.Media;
using System;

namespace MapsControl.Presentation
{
    public class MapEntityView : IMapEntityView
    {
        #region Fields

        protected readonly FrameworkElement _visualElement;

        #endregion

        #region Properties

        private TranslateTransform TranslateTransform
        {
            get { return (TranslateTransform)_visualElement.RenderTransform; }
        }

        public FrameworkElement VisualRoot
        {
            get { return _visualElement; }
        }

        public Point Offset
        {
            get { return new Point(TranslateTransform.X, TranslateTransform.Y); }
            set
            {
                TranslateTransform.X = value.X;
                TranslateTransform.Y = value.Y;
            }
        }

        #endregion

        #region Constructors

        public MapEntityView(FrameworkElement frameworkElement)
        {
            _visualElement = frameworkElement;
        }

        #endregion
    }
}