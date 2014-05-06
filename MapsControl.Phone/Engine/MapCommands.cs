using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
#if DESKTOP
using System.Reactive.Subjects;
#endif
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MapsControl.Presentation;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace MapsControl.Engine
{
    public class MapCommands : IMapCommands
    {
        #region Subjects

        public ISubject<GeoCoordinate> GeoCoordinateCentersSubject { get; set; }

        public ISubject<double> ZoomsSubject { get; set; }

        public ISubject<ITileSourceProvider> TileUriProvidersSubject { get; set; }

        #endregion

        #region IMapCommands
        
        public IObservable<EventArgs> Initialized { get; set; }

        public IObservable<Size> SizeChanges { get; set; }

        public IObservable<Point2D> Translations { get; set; }

        public IObservable<GeoCoordinate> GeoCoordinateCenters { get { return GeoCoordinateCentersSubject; } }

        public IObservable<double> Zooms { get { return ZoomsSubject; } }

        public IObservable<ITileSourceProvider> TileUriProviders { get { return TileUriProvidersSubject; } }

        public IObservable<IEnumerable<IMapEntityView>> EntityViewAdded { get; set; }

        #endregion

        #region Constructors

        public MapCommands()
        {
            GeoCoordinateCentersSubject = new Subject<GeoCoordinate>();
            ZoomsSubject = new Subject<double>();
            TileUriProvidersSubject = new Subject<ITileSourceProvider>();
        }

        #endregion
    }
}
