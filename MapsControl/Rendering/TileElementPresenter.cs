using System.ComponentModel;
using MapsControl.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapsControl.Rendering
{
    public abstract class TileElementPresenter : IDisposable, ITileElementPresenter
    {
        #region Properties

        public UIElement VisualElement { get; protected set; }
        public Tile Tile { get; private set; }

        #endregion

        #region Constructors

        protected TileElementPresenter(Tile tile)
        {
            Tile = tile;
            tile.PropertyChanged += OnTilePropertyChanged;;
        }

        private void OnTilePropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "OffsetX" 
                || propertyChangedEventArgs.PropertyName == "OffsetY")
            {
                OnTileOffsetChanged();
            }

            if (propertyChangedEventArgs.PropertyName == "Uri")
            {
                OnTileUriChanged();
            }
        }

        #endregion

        #region Methods

        protected abstract void OnTileUriChanged();

        protected abstract void OnTileOffsetChanged();

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Tile.PropertyChanged -= OnTilePropertyChanged;
        }

        #endregion
    }
}
