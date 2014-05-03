using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MapsControl.Presentation;

namespace MapsControl.Engine
{
    public interface IMapCommands
    {
        IObservable<EventArgs> Initialized { get; }
        IObservable<Size> SizeChanges { get; }
        IObservable<Point2D> Translations { get; }
        IObservable<GeoCoordinate> GeoCoordinateCenters { get; }
        IObservable<double> Zooms { get; }
        IObservable<ITileUriProvider> TileUriProviders { get; }
        IObservable<IEnumerable<IMapEntityView>> EntityViewAdded { get; }
    }
}
