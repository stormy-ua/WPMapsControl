using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Windows;
using MapsControl.Engine;

namespace MapsControl.Presentation
{
    public interface IMapView
    {
        void Add(IMapEntityView mapEntityView);
        void Remove(IMapEntityView mapEntityView);        
    }
}