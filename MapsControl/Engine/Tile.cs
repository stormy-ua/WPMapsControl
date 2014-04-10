using System;

namespace MapsControl.Engine
{
    public class Tile : MapEntity
    {
        #region Fields

        private Uri _uri;

        #endregion

        #region Properties

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
    }   
}