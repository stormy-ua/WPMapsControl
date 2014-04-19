using System;
using System.ComponentModel;
using MapsControl.Engine;

namespace MapsControl.Presentation
{
    public class MapOverlayPresenter : MapEntityPresenter
    {
        #region Constructors

        public MapOverlayPresenter(IMapController mapController, IMapOverlayView view, Pin pin)
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
            _mapEntityView.OffsetY -= _mapEntityView.VisualRoot.Height;
        }

        #endregion
    }
}