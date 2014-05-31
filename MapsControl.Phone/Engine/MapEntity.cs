using System;
using System.CodeDom;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif
#if DESKTOP
using System.Reactive.Subjects;
#endif

namespace MapsControl.Engine
{
    public class MapEntity
    {
        #region Fields

        private readonly ISubject<Point> _offsetChangesSubject = new Subject<Point>();
        private Point _offset;

        #endregion

        #region Properties

        public IObservable<Point> OffsetChanges { get { return _offsetChangesSubject; } }

        public int MapX { get; set; }

        public int MapY { get; set; }

        public Point Offset
        {
            get { return _offset; }
            set
            {
                if (_offset != value)
                {
                    _offset = value;
                    _offsetChangesSubject.OnNext(_offset);
                }
            }
        }

        #endregion
    }
}