using System;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif
#if DESKTOP
using System.Reactive.Subjects;
#endif

namespace MapsControl.Engine
{
    public class Tile : MapEntity
    {
        #region Fields

        private readonly ISubject<TileSource> _tileSourceChangesSubject = new Subject<TileSource>();
        private TileSource _tileSource;

        #endregion

        #region Properties

        public IObservable<TileSource> TileSourceChanges { get { return _tileSourceChangesSubject; } } 

        public TileSource TileSource
        {
            get { return _tileSource; }
            set
            {
                if (_tileSource != value)
                {
                    _tileSource = value;
                    _tileSourceChangesSubject.OnNext(_tileSource);
                }
            }
        }

        public double LevelOfDetails { get; set; }

        #endregion
    }   
}