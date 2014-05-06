using MapsControl.Engine;
using MapsControl.Presentation;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MapsControl.Desktop
{
    public class MapsControl : MapsControlBase
    {
        #region Constructors

        public MapsControl()
            : base()
        {
            DefaultStyleKey = typeof(MapsControl);

            _mapCommands.Translations = Observable.FromEventPattern<ManipulationDeltaEventArgs>(this, "ManipulationDelta")
                      .Select(args => args.EventArgs.DeltaManipulation.Translation.ToPoint2D());
            _mapCommands.SizeChanges = Observable.FromEventPattern<SizeChangedEventArgs>(this, "SizeChanged")
                      .Select(args => args.EventArgs.NewSize);
            _mapCommands.Initialized = Observable.FromEventPattern<RoutedEventArgs>(this, "Loaded")
                      .Select(args => args.EventArgs);

            _mapCommands.SizeChanges.Subscribe(size => _panel.Clip = new RectangleGeometry { Rect = new Rect(0, 0, size.Width, size.Height) });
            _mapCommands.EntityViewAdded = Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(MapElements, "CollectionChanged")
                                        .Where(args => args.EventArgs.Action == NotifyCollectionChangedAction.Add)
                                        .Select(args => args.EventArgs.NewItems.OfType<IMapEntityView>())
                                        .Where(entityView => entityView != null && entityView.Any());

            var mouseDownPoints = Observable.FromEventPattern<MouseButtonEventArgs>(this, "MouseLeftButtonDown");
            var mouseUpPoints = Observable.FromEventPattern<MouseButtonEventArgs>(this, "MouseLeftButtonUp");
            var mouseMovePoints = Observable.FromEventPattern<MouseEventArgs>(this, "MouseMove")
                .Select(e => e.EventArgs.GetPosition(null))
                .SkipUntil(mouseDownPoints)
                .TakeUntil(mouseUpPoints);
            _mapCommands.Translations = mouseMovePoints
                .Skip(1)
                .CombineLatest(mouseMovePoints, (point2, point1) => (point2 - point1).ToPoint2D())
                .Repeat();

            _mapPresenter = new MapPresenter(this, _mapCommands, 5);
        }

        #endregion
    }
}
