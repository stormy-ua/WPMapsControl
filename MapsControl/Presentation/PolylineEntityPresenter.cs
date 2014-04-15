using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MapsControl.Engine;
using MapsControl.Infrastructure;

namespace MapsControl.Presentation
{
    public class PolylineEntityPresenter
    {
        private readonly IPolylineEntityView _view;
        private readonly PolylineEntity _polylineEntity;
        private readonly IMapController _mapController;

        public PolylineEntityPresenter(IMapController mapController, IPolylineEntityView view,
            PolylineEntity polylineEntity)
        {
            _view = view;
            _polylineEntity = polylineEntity;
            _mapController = mapController;

            polylineEntity.Pins.ForEach(AttachPin);
            polylineEntity.Pins.CollectionChanged += (sender, args) =>
            {
                if (args.Action != NotifyCollectionChangedAction.Add)
                {
                    return;
                }

                args.NewItems.OfType<Pin>().ForEach(AttachPin);
            };
        }

        private void OnPinOffsetChanged(int index, Pin pin)
        {
            _view[index] = new Point(pin.OffsetX, pin.OffsetY);
        }

        private void AttachPin(Pin pin)
        {
            _mapController.PositionPin(pin);
            OnPinOffsetChanged(_polylineEntity.Pins.IndexOf(pin), pin);

            pin.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "OffsetX"
                    || args.PropertyName == "OffsetY")
                {
                    var p = (Pin)sender;
                    OnPinOffsetChanged(_polylineEntity.Pins.IndexOf(pin), p);
                }
            };
        }
    }
}
