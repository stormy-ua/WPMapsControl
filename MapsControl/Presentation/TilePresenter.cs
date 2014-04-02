using System;
using System.ComponentModel;
using System.Windows;
using MapsControl.Engine;
using MapsControl.Rendering;

namespace MapsControl.Presentation
{
    public class TilePresenter : IDisposable, ITilePresenter
    {
        #region Fields

        private readonly ITileView _tileView;
        private readonly Tile _tile;

        #endregion

        #region Constructors

        public TilePresenter(ITileView tileView, Tile tile)
        {
            _tileView = tileView;
            _tile = tile;
            tile.PropertyChanged += OnTilePropertyChanged;
            OnTileOffsetChanged();
            OnTileUriChanged();
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

        private void OnTileUriChanged()
        {
            _tileView.Uri = _tile.Uri;
        }

        private void OnTileOffsetChanged()
        {
            _tileView.OffsetX = _tile.OffsetX;
            _tileView.OffsetY = _tile.OffsetY;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _tile.PropertyChanged -= OnTilePropertyChanged;
        }

        #endregion
    }
}
