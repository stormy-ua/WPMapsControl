using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MapsControl.Engine
{
    public class Tile : INotifyPropertyChanged
    {
        #region Fields

        private int _mapX;
        private int _mapY;
        private double _offsetX;
        private double _offsetY;
        private Uri _uri;

        #endregion

        #region Properties

        public int MapX
        {
            get { return _mapX; }
            set
            {
                if (_mapX != value)
                {
                    _mapX = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MapY
        {
            get { return _mapY; }
            set
            {
                if (_mapY != value)
                {
                    _mapY = value;
                    OnPropertyChanged();
                }
            }
        }

        public double OffsetX
        {
            get { return _offsetX; }
            set
            {
                if (_offsetX != value)
                {
                    _offsetX = value;
                    OnPropertyChanged();
                }
            }
        }

        public double OffsetY
        {
            get { return _offsetY; }
            set
            {
                if (_offsetY != value)
                {
                    _offsetY = value;
                    OnPropertyChanged();
                }
            }
        }

        public Uri Uri
        {
            get { return _uri; }
            set
            {
                if (_uri != value)
                {
                    _uri = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }   
}