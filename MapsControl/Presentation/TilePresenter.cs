using System.ComponentModel;
using System.Windows;
using MapsControl.Engine;
using MapsControl.Presentation;

namespace MapsControl.Presentation
{
    public class TilePresenter : MapEntityPresenter
    {
        #region Constructors

        public TilePresenter(ITileView tileView, Tile tile)
            : base(tileView, tile)
        {
            OnTileUriChanged();
        }

        protected override void OnMapEntityPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnMapEntityPropertyChanged(sender, propertyChangedEventArgs);

            if (propertyChangedEventArgs.PropertyName == "Uri")
            {
                OnTileUriChanged();
            }
        }

        #endregion

        #region Methods

        private void OnTileUriChanged()
        {
            ((ITileView)_mapEntityView).Uri = ((Tile)_mapEntity).Uri;
        }

        #endregion
    }
}
