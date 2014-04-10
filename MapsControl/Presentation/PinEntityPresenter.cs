using System;
using System.ComponentModel;
using MapsControl.Engine;

namespace MapsControl.Presentation
{
    public class PinEntityPresenter : MapEntityPresenter
    {
        #region Constructors

        public PinEntityPresenter(IMapEntityView mapEntityView, Pin pin)
            : base(mapEntityView, pin)
        {
            OnGeoCoordinateChanged();
        }

        #endregion

        #region Methods

        protected override void OnMapEntityPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnMapEntityPropertyChanged(sender, propertyChangedEventArgs);

            if (propertyChangedEventArgs.PropertyName == "GeoCoordinate")
            {
                OnGeoCoordinateChanged();
            }
        }

        private void OnGeoCoordinateChanged()
        {
        }

        protected override void OnOffsetChanged()
        {
            _mapEntityView.OffsetX = _mapEntity.OffsetX;
            _mapEntityView.OffsetY = _mapEntity.OffsetY - _mapEntityView.VisualRoot.Height;
        }

        #endregion
    }
}