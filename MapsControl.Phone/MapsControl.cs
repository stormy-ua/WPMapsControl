using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MapsControl.Engine;
using MapsControl.Presentation;
using Microsoft.Phone.Reactive;

namespace MapsControl
{
    public class MapsControl : MapsControlBase
    {
        #region Constructors

        public MapsControl()
            : base()
        {
            DefaultStyleKey = typeof(MapsControl);

            _mapCommands.Translations = Observable.FromEvent<ManipulationDeltaEventArgs>(this, "ManipulationDelta")
                      .Select(args => args.EventArgs.DeltaManipulation.Translation.ToPoint2D());
            _mapCommands.SizeChanges = Observable.FromEvent<SizeChangedEventArgs>(this, "SizeChanged")
                      .Select(args => args.EventArgs.NewSize);
            _mapCommands.Initialized = Observable.FromEvent<RoutedEventArgs>(this, "Loaded")
                      .Select(args => args.EventArgs);

            _mapCommands.SizeChanges.Subscribe(size => _panel.Clip = new RectangleGeometry { Rect = new Rect(0, 0, size.Width, size.Height) });
            _mapCommands.EntityViewAdded = Observable.FromEvent<NotifyCollectionChangedEventArgs>(MapElements, "CollectionChanged")
                                        .Where(args => args.EventArgs.Action == NotifyCollectionChangedAction.Add)
                                        .Select(args => args.EventArgs.NewItems.OfType<IMapEntityView>())
                                        .Where(entityView => entityView != null && entityView.Any());

            _mapPresenter = new MapPresenter(this, _mapCommands);
        }

        #endregion
    }
}
