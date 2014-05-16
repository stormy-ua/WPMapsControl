using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Windows;
using MapsControl.Engine;

namespace MapsControl.Presentation
{
    public interface IMapView
    {
        GeoCoordinate GeoCoordinateCenter { get; set; }

        void Add(IMapEntityView mapEntityView, XMapLayer layer);
        void Remove(IMapEntityView mapEntityView);        
    }
}