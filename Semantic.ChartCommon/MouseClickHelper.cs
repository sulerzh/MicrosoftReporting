using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class MouseClickHelper
    {
        private static readonly TimeSpan MaxClickTimeInMilliseconds = TimeSpan.FromMilliseconds(5000.0);
        private List<MouseClickHelper.MouseEventInfo> _queuedMouseDownEvents = new List<MouseClickHelper.MouseEventInfo>();
        private List<MouseClickHelper.MouseEventInfo> _queuedMouseUpEvents = new List<MouseClickHelper.MouseEventInfo>();
        private const double DefaultDoubleClickInMilliseconds = 150.0;
        private const double MaxDistance = 5.0;
        private TimeSpan _doubleClickTime;
        private Action<object, MouseButtonEventArgs> _singleClickAction;
        private Action<object, MouseButtonEventArgs> _doubleClickAction;
        private DispatcherTimer _timer;

        private DispatcherTimer Timer
        {
            get
            {
                if (this._timer == null)
                {
                    this._timer = new DispatcherTimer();
                    this._timer.Interval = this._doubleClickTime;
                    this._timer.Tick += new EventHandler(this.Timer_Tick);
                }
                return this._timer;
            }
        }

        private bool IsQueueEmpty
        {
            get
            {
                if (this._queuedMouseDownEvents.Count == 0)
                    return this._queuedMouseUpEvents.Count == 0;
                return false;
            }
        }

        public MouseClickHelper(Action<object, MouseButtonEventArgs> onSingleClickAction = null, Action<object, MouseButtonEventArgs> onDoubleClickAction = null, TimeSpan? doubleClickTime = null)
        {
            this._singleClickAction = onSingleClickAction;
            this._doubleClickAction = onDoubleClickAction;
            this._doubleClickTime = doubleClickTime ?? TimeSpan.FromMilliseconds(150.0);
        }

        public void Attach(UIElement element)
        {
            element.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnMouseDown);
            element.MouseMove += new MouseEventHandler(this.OnMouseMove);
            element.MouseLeftButtonUp += new MouseButtonEventHandler(this.OnMouseUp);
        }

        public void Detach(UIElement element)
        {
            element.MouseLeftButtonDown -= new MouseButtonEventHandler(this.OnMouseDown);
            element.MouseMove -= new MouseEventHandler(this.OnMouseMove);
            element.MouseLeftButtonUp -= new MouseButtonEventHandler(this.OnMouseUp);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DateTime now = DateTime.Now;
            MouseClickHelper.MouseEventInfo @event = this.FindEvent(this._queuedMouseUpEvents, sender);
            if (@event != null)
                @event.DateTime = now;
            this.RemoveEvent(this._queuedMouseDownEvents, sender);
            this.AddEvent(this._queuedMouseDownEvents, sender, e);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseClickHelper.MouseEventInfo @event = this.FindEvent(this._queuedMouseUpEvents, sender);
            if (@event == null)
            {
                this.AddEvent(this._queuedMouseUpEvents, sender, e);
            }
            else
            {
                this.RemoveEvent(this._queuedMouseUpEvents, @event);
                this.RemoveEvent(this._queuedMouseDownEvents, sender);
                if (this._doubleClickAction == null)
                    return;
                this._doubleClickAction(sender, e);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsQueueEmpty)
                return;
            MouseClickHelper.MouseEventInfo @event = this.FindEvent(this._queuedMouseUpEvents, sender);
            if (@event == null || MouseClickHelper.MouseEventInfo.IsInProximity(e.GetPosition(sender as UIElement), @event.Position))
                return;
            this.FireSingleClick(@event);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            foreach (MouseClickHelper.MouseEventInfo mouseEventInfo in this._queuedMouseDownEvents.ToArray())
            {
                if (now.Subtract(mouseEventInfo.DateTime) > MouseClickHelper.MaxClickTimeInMilliseconds)
                    this._queuedMouseDownEvents.Remove(mouseEventInfo);
            }
            foreach (MouseClickHelper.MouseEventInfo info in this._queuedMouseUpEvents.ToArray())
            {
                if (now.Subtract(info.DateTime) > this._doubleClickTime)
                    this.FireSingleClick(info);
            }
            if (!this.IsQueueEmpty)
                return;
            this.Timer.Stop();
        }

        private void FireSingleClick(MouseClickHelper.MouseEventInfo info)
        {
            this._queuedMouseUpEvents.Remove(info);
            MouseClickHelper.MouseEventInfo @event = this.FindEvent(this._queuedMouseDownEvents, info.Sender);
            if (@event == null)
                return;
            this._queuedMouseDownEvents.Remove(@event);
            if (!info.IsInProximity(@event) || this._singleClickAction == null)
                return;
            this._singleClickAction(info.Sender, info.Args);
        }

        private void AddEvent(List<MouseClickHelper.MouseEventInfo> queuedEvents, object sender, MouseButtonEventArgs args)
        {
            queuedEvents.Add(new MouseClickHelper.MouseEventInfo()
            {
                Sender = sender,
                Args = args,
                DateTime = DateTime.Now
            });
            if (this.Timer.IsEnabled)
                return;
            this.Timer.Start();
        }

        private MouseClickHelper.MouseEventInfo FindEvent(List<MouseClickHelper.MouseEventInfo> queuedEvents, object sender)
        {
            return queuedEvents.Where<MouseClickHelper.MouseEventInfo>(info => info.Sender == sender).FirstOrDefault<MouseClickHelper.MouseEventInfo>();
        }

        private void RemoveEvent(List<MouseClickHelper.MouseEventInfo> queuedEvents, object sender)
        {
            MouseClickHelper.MouseEventInfo @event = this.FindEvent(queuedEvents, sender);
            this.RemoveEvent(queuedEvents, @event);
        }

        private void RemoveEvent(List<MouseClickHelper.MouseEventInfo> queuedEvents, MouseClickHelper.MouseEventInfo prevEvent)
        {
            if (prevEvent == null)
                return;
            queuedEvents.Remove(prevEvent);
        }

        private class MouseEventInfo
        {
            public object Sender { get; set; }

            public MouseButtonEventArgs Args { get; set; }

            public DateTime DateTime { get; set; }

            public Point Position
            {
                get
                {
                    return this.Args.GetPosition(this.Sender as UIElement);
                }
            }

            public bool IsInProximity(MouseClickHelper.MouseEventInfo another)
            {
                return MouseClickHelper.MouseEventInfo.IsInProximity(this.Position, another.Position);
            }

            public static bool IsInProximity(Point one, Point another)
            {
                if (Math.Abs(one.X - another.X) < 5.0)
                    return Math.Abs(one.Y - another.Y) < 5.0;
                return false;
            }
        }
    }
}
