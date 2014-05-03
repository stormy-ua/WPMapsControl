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

        public double OffsetX
        {
            get { return TranslateTransform.X; }
            set { TranslateTransform.X = value; }
        }

        public double OffsetY
        {
            get { return TranslateTransform.Y; }
            set { TranslateTransform.Y = value; }
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