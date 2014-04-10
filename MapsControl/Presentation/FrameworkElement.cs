using System.Windows;
using System.Windows.Media;

namespace MapsControl.Presentation
{
    public class FrameworkElementView : IMapEntityView
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

        public FrameworkElementView(FrameworkElement visualElement)
        {
            _visualElement = visualElement;
        }

        #endregion
    }
}