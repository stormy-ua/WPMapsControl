using System;
using System.ComponentModel;
using MapsControl.Engine;

namespace MapsControl.Presentation
{
    public class MapEntityPresenter : IDisposable
    {
        #region Fields

        protected readonly IMapEntityView _mapEntityView;
        protected readonly MapEntity _mapEntity;

        #endregion

        #region Properties

        public IMapEntityView View
        {
            get { return _mapEntityView; }
        }

        public MapEntity MapEntity
        {
            get { return _mapEntity; }
        }

        #endregion

        #region Constructors

        public MapEntityPresenter(IMapEntityView mapEntityView, MapEntity mapEntity)
        {
            _mapEntityView = mapEntityView;
            _mapEntity = mapEntity;
            mapEntity.PropertyChanged += OnMapEntityPropertyChanged;
            _mapEntityView.OffsetX = _mapEntity.OffsetX;
            _mapEntityView.OffsetY = _mapEntity.OffsetY;
        }

        protected virtual void OnMapEntityPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "OffsetX"
                || propertyChangedEventArgs.PropertyName == "OffsetY")
            {
                OnOffsetChanged();
            }
        }

        #endregion

        #region Methods

        protected virtual void OnOffsetChanged()
        {
            _mapEntityView.OffsetX = _mapEntity.OffsetX;
            _mapEntityView.OffsetY = _mapEntity.OffsetY;
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            _mapEntity.PropertyChanged -= OnMapEntityPropertyChanged;
        }

        #endregion
    }
}