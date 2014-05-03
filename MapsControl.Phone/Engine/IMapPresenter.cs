using System.Collections.Generic;
using System.Device.Location;
using System.Windows;
using MapsControl.Presentation;

namespace MapsControl.Engine
{
    public interface IMapPresenter
    {
        void PositionPin(Pin pin);
    }
}