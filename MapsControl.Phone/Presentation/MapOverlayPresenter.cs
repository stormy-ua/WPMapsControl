using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MapsControl.Engine;

namespace MapsControl.Presentation
{
    public class MapOverlayPresenter : MapEntityPresenter
    {
        #region Constructors

        public MapOverlayPresenter(IMapPresenter mapController, IMapOverlayView view, Pin pin)
            : base(view, pin)
        {
            mapController.PositionPin(pin);
            view.GeoCoordinateChanged += (sender, coordinate) =>
            {
                pin.GeoCoordinate = coordinate;
                mapController.PositionPin(pin);
            };
        }

        #endregion

        #region Methods

        protected override void OnOffsetChanged()
        {
            base.OnOffsetChanged();
            //_mapEntityView.OffsetY -= ((FrameworkElement)((ContentControl)_mapEntityView.VisualRoot).Content).Height;
        }

        #endregion
    }
}