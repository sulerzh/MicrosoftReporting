using System;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    [TemplatePart(Name = "HorizontalThumbResizeDecrease", Type = typeof(Thumb))]
    [TemplatePart(Name = "VerticalThumbResizeIncrease", Type = typeof(Thumb))]
    [TemplatePart(Name = "Root", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "HorizontalRoot", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "HorizontalThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "HorizontalSmallDecrease", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "HorizontalLargeIncrease", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "HorizontalLargeDecrease", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "HorizontalSmallIncrease", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "HorizontalThumbResizeIncrease", Type = typeof(Thumb))]
    [TemplateVisualState(GroupName = "MinimizeMaximizeStateGroup", Name = "MinimizeState")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplatePart(Name = "VerticalThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "VerticalSmallIncrease", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "VerticalLargeIncrease", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "VerticalLargeDecrease", Type = typeof(RepeatButton))]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
    [TemplatePart(Name = "VerticalSmallDecrease", Type = typeof(RepeatButton))]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplatePart(Name = "VerticalThumbResizeDecrease", Type = typeof(Thumb))]
    [TemplatePart(Name = "VerticalRoot", Type = typeof(FrameworkElement))]
    [TemplateVisualState(GroupName = "MinimizeMaximizeStateGroup", Name = "MaximizeState")]
    public class ScrollZoomBar : RangeBase
    {
        public static readonly DependencyProperty ViewportSizeProperty = DependencyProperty.Register("ViewportSize", typeof(double), typeof(ScrollZoomBar), new PropertyMetadata(0.0, new PropertyChangedCallback(ScrollZoomBar.OnViewportSizePropertyChanged)));
        public static readonly DependencyProperty MinimumViewportSizeProperty = DependencyProperty.Register("MinimumViewportSize", typeof(double), typeof(ScrollZoomBar), new PropertyMetadata(0.001, new PropertyChangedCallback(ScrollZoomBar.OnMinimumViewportSizePropertyChanged)));
        public static readonly DependencyProperty IsResizingProperty = DependencyProperty.Register("IsResizing", typeof(bool), typeof(ScrollZoomBar), new PropertyMetadata(false, new PropertyChangedCallback(ScrollZoomBar.OnIsResizingPropertyChanged)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ScrollZoomBar), new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(ScrollZoomBar.OnOrientationPropertyChanged)));
        public static readonly DependencyProperty AutoAdjustMaximumProperty = DependencyProperty.Register("AutoAdjustMaximum", typeof(bool), typeof(ScrollZoomBar), new PropertyMetadata(true, new PropertyChangedCallback(ScrollZoomBar.OnAutoAdjustMaximumPropertyChanged)));
        public static readonly DependencyProperty MinimizedSizeProperty = DependencyProperty.Register("MinimizedSize", typeof(double), typeof(ScrollZoomBar), new PropertyMetadata(7.0, new PropertyChangedCallback(ScrollZoomBar.OnMinimizedSizePropertyChanged)));
        public static readonly DependencyProperty MaximizedSizeProperty = DependencyProperty.Register("MaximizedSize", typeof(double), typeof(ScrollZoomBar), new PropertyMetadata(17.0, new PropertyChangedCallback(ScrollZoomBar.OnMaximizedSizePropertyChanged)));
        public static readonly DependencyProperty IsAllwaysMaximizedProperty = DependencyProperty.Register("IsAllwaysMaximized", typeof(bool), typeof(ScrollZoomBar), new PropertyMetadata(false, new PropertyChangedCallback(ScrollZoomBar.OnIsAllwaysMaximizedPropertyChanged)));
        public static readonly DependencyProperty ResizeViewHandlesVisibilityProperty = DependencyProperty.Register("ResizeViewHandlesVisibility", typeof(Visibility), typeof(ScrollZoomBar), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(ScrollZoomBar.OnResizeViewHandlesVisibilityPropertyChanged)));
        public static readonly DependencyProperty ValueDragIntervalProperty = DependencyProperty.Register("ValueDragInterval", typeof(long), typeof(ScrollZoomBar), new PropertyMetadata(5L, new PropertyChangedCallback(ScrollZoomBar.OnValueDragIntervalPropertyChanged)));
        private double _pendingValue = double.NaN;
        private const string ElementRootTemplateName = "Root";
        private const string ElementHorizontalTemplateName = "HorizontalRoot";
        private const string ElementHorizontalThumbName = "HorizontalThumb";
        private const string ElementHorizontalLargeDecreaseName = "HorizontalLargeDecrease";
        private const string ElementHorizontalLargeIncreaseName = "HorizontalLargeIncrease";
        private const string ElementHorizontalSmallDecreaseName = "HorizontalSmallDecrease";
        private const string ElementHorizontalSmallIncreaseName = "HorizontalSmallIncrease";
        private const string ElementHorizontalThumbResizeIncreaseName = "HorizontalThumbResizeIncrease";
        private const string ElementHorizontalThumbResizeDecreaseName = "HorizontalThumbResizeDecrease";
        private const string ElementVerticalTemplateName = "VerticalRoot";
        private const string ElementVerticalThumbName = "VerticalThumb";
        private const string ElementVerticalLargeDecreaseName = "VerticalLargeDecrease";
        private const string ElementVerticalLargeIncreaseName = "VerticalLargeIncrease";
        private const string ElementVerticalSmallDecreaseName = "VerticalSmallDecrease";
        private const string ElementVerticalSmallIncreaseName = "VerticalSmallIncrease";
        private const string ElementVerticalThumbResizeIncreaseName = "VerticalThumbResizeIncrease";
        private const string ElementVerticalThumbResizeDecreaseName = "VerticalThumbResizeDecrease";
        private const string GroupCommonStateName = "CommonStates";
        private const string StateDisabledName = "Disabled";
        private const string StateMouseOverName = "MouseOver";
        private const string StateNormalName = "Normal";
        private const string GroupMinimizeMaximizeStateName = "MinimizeMaximizeStateGroup";
        private const string StateMinimizeName = "MinimizeState";
        private const string StateMaximizeName = "MaximizeState";
        internal const string ViewportSizePropertyName = "ViewportSize";
        internal const string MinimumViewportSizePropertyName = "MinimumViewportSize";
        internal const string IsResizingPropertyName = "IsResizing";
        internal const string OrientationPropertyName = "Orientation";
        internal const string AutoAdjustMaximumPropertyName = "AutoAdjustMaximum";
        internal const string MinimizedSizePropertyName = "MinimizedSize";
        internal const string MaximizedSizePropertyName = "MaximizedSize";
        internal const string IsAllwaysMaximizedPropertyName = "IsAllwaysMaximized";
        internal const string ResizeViewHandlesVisibilityPropertyName = "ResizeViewHandlesVisibility";
        internal const string ValueDragIntervalPropertyName = "ValueDragInterval";
        private FrameworkElement _elementRootTemplate;
        private RepeatButton _elementHorizontalLargeDecrease;
        private RepeatButton _elementHorizontalLargeIncrease;
        private RepeatButton _elementHorizontalSmallDecrease;
        private RepeatButton _elementHorizontalSmallIncrease;
        private FrameworkElement _elementHorizontalTemplate;
        private Thumb _elementHorizontalThumb;
        private Thumb _elementHorizontalThumbResizeInc;
        private Thumb _elementHorizontalThumbResizeDec;
        private RepeatButton _elementVerticalLargeDecrease;
        private RepeatButton _elementVerticalLargeIncrease;
        private RepeatButton _elementVerticalSmallDecrease;
        private RepeatButton _elementVerticalSmallIncrease;
        private FrameworkElement _elementVerticalTemplate;
        private Thumb _elementVerticalThumb;
        private Thumb _elementVerticalThumbResizeInc;
        private Thumb _elementVerticalThumbResizeDec;
        private VisualState _elementMinimizeVisualState;
        private VisualState _elementMaximizeVisualState;
        private bool _isMouseOver;
        private double _maximumAbsolute;
        private bool _isAutoadjusting;
        private DispatcherTimer _timer;
        private double _startDragViewportValue;
        private double _currentDragViewportValue;
        private double _dragValue;

        public double ViewportSize
        {
            get
            {
                return (double)this.GetValue(ScrollZoomBar.ViewportSizeProperty);
            }
            set
            {
                this.SetValue(ScrollZoomBar.ViewportSizeProperty, value);
            }
        }

        public double MinimumViewportSize
        {
            get
            {
                return (double)this.GetValue(ScrollZoomBar.MinimumViewportSizeProperty);
            }
            set
            {
                this.SetValue(ScrollZoomBar.MinimumViewportSizeProperty, value);
            }
        }

        public bool IsResizing
        {
            get
            {
                return (bool)this.GetValue(ScrollZoomBar.IsResizingProperty);
            }
            private set
            {
                this.SetValue(ScrollZoomBar.IsResizingProperty, value);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(ScrollZoomBar.OrientationProperty);
            }
            set
            {
                this.SetValue(ScrollZoomBar.OrientationProperty, value);
            }
        }

        public bool AutoAdjustMaximum
        {
            get
            {
                return (bool)this.GetValue(ScrollZoomBar.AutoAdjustMaximumProperty);
            }
            set
            {
                this.SetValue(ScrollZoomBar.AutoAdjustMaximumProperty, value);
            }
        }

        public double MinimizedSize
        {
            get
            {
                return (double)this.GetValue(ScrollZoomBar.MinimizedSizeProperty);
            }
            set
            {
                this.SetValue(ScrollZoomBar.MinimizedSizeProperty, value);
            }
        }

        public double MaximizedSize
        {
            get
            {
                return (double)this.GetValue(ScrollZoomBar.MaximizedSizeProperty);
            }
            set
            {
                this.SetValue(ScrollZoomBar.MaximizedSizeProperty, value);
            }
        }

        public bool IsAllwaysMaximized
        {
            get
            {
                return (bool)this.GetValue(ScrollZoomBar.IsAllwaysMaximizedProperty);
            }
            set
            {
                this.SetValue(ScrollZoomBar.IsAllwaysMaximizedProperty, value);
            }
        }

        public Visibility ResizeViewHandlesVisibility
        {
            get
            {
                return (Visibility)this.GetValue(ScrollZoomBar.ResizeViewHandlesVisibilityProperty);
            }
            set
            {
                this.SetValue(ScrollZoomBar.ResizeViewHandlesVisibilityProperty, value);
            }
        }

        public long ValueDragInterval
        {
            get
            {
                return (long)this.GetValue(ScrollZoomBar.ValueDragIntervalProperty);
            }
            set
            {
                this.SetValue(ScrollZoomBar.ValueDragIntervalProperty, value);
            }
        }

        public event ScrollEventHandler Scroll;

        public event RoutedPropertyChangedEventHandler<double> ViewportSizeChanged;

        public event RoutedPropertyChangedEventHandler<double> ViewportSizeChangeCompleted;

        public event RoutedPropertyChangedEventHandler<Orientation> OrientationChanged;

        public ScrollZoomBar()
        {
            this.IsEnabledChanged += (s, e) => this.OnIsEnabledChanged();
            this.SizeChanged += (s, e) => this.UpdateTrackLayout(this.GetTrackLength());
            this.DefaultStyleKey = typeof(ScrollZoomBar);
            this._maximumAbsolute = this.Maximum;
        }

        private static void OnViewportSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScrollZoomBar)d).OnViewportSizePropertyChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void OnViewportSizePropertyChanged(double oldValue, double newValue)
        {
            if (this.AutoAdjustMaximum)
            {
                this._isAutoadjusting = true;
                try
                {
                    this.Maximum = (double)(this._maximumAbsolute - (DoubleR10)newValue);
                }
                finally
                {
                    this._isAutoadjusting = false;
                }
            }
            if (this.ViewportSizeChanged != null && oldValue != newValue)
                this.ViewportSizeChanged(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
            if (this.AutoAdjustMaximum)
                return;
            this.UpdateTrackLayout(this.GetTrackLength());
        }

        private static void OnMinimumViewportSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScrollZoomBar)d).OnMinimumViewportSizePropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnMinimumViewportSizePropertyChanged(object oldValue, object newValue)
        {
        }

        private static void OnIsResizingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScrollZoomBar)d).OnIsResizingPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsResizingPropertyChanged(object oldValue, object newValue)
        {
        }

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScrollZoomBar)d).OnOrientationPropertyChanged((Orientation)e.OldValue, (Orientation)e.NewValue);
        }

        protected virtual void OnOrientationPropertyChanged(Orientation oldValue, Orientation newValue)
        {
            double trackLength = this.GetTrackLength();
            if (this._elementHorizontalTemplate != null)
                this._elementHorizontalTemplate.Visibility = this.Orientation == Orientation.Horizontal ? Visibility.Visible : Visibility.Collapsed;
            if (this._elementVerticalTemplate != null)
                this._elementVerticalTemplate.Visibility = this.Orientation == Orientation.Horizontal ? Visibility.Collapsed : Visibility.Visible;
            this.UpdateTrackLayout(trackLength);
            if (this.OrientationChanged == null || oldValue == newValue)
                return;
            this.OrientationChanged(this, new RoutedPropertyChangedEventArgs<Orientation>(oldValue, newValue));
        }

        private static void OnAutoAdjustMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScrollZoomBar)d).OnAutoAdjustMaximumPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnAutoAdjustMaximumPropertyChanged(object oldValue, object newValue)
        {
        }

        private static void OnMinimizedSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScrollZoomBar)d).OnMinimizedSizePropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnMinimizedSizePropertyChanged(object oldValue, object newValue)
        {
            this.SetGrowAnimationParams(this._elementMinimizeVisualState, this.MinimizedSize);
        }

        private static void OnMaximizedSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScrollZoomBar)d).OnMaximizedSizePropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnMaximizedSizePropertyChanged(object oldValue, object newValue)
        {
            this.SetGrowAnimationParams(this._elementMaximizeVisualState, this.MaximizedSize);
        }

        private static void OnIsAllwaysMaximizedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScrollZoomBar)d).OnIsAllwaysMaximizedPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsAllwaysMaximizedPropertyChanged(object oldValue, object newValue)
        {
            this.UpdateVisualState(false);
        }

        private static void OnResizeViewHandlesVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScrollZoomBar)d).OnResizeViewHandlesVisibilityPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnResizeViewHandlesVisibilityPropertyChanged(object oldValue, object newValue)
        {
        }

        private static void OnValueDragIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScrollZoomBar)d).OnValueDragIntervalPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnValueDragIntervalPropertyChanged(object oldValue, object newValue)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._elementRootTemplate = this.GetTemplateChild("Root") as FrameworkElement;
            this._elementHorizontalTemplate = this.GetTemplateChild("HorizontalRoot") as FrameworkElement;
            this._elementHorizontalLargeIncrease = this.GetTemplateChild("HorizontalLargeIncrease") as RepeatButton;
            this._elementHorizontalLargeDecrease = this.GetTemplateChild("HorizontalLargeDecrease") as RepeatButton;
            this._elementHorizontalSmallIncrease = this.GetTemplateChild("HorizontalSmallIncrease") as RepeatButton;
            this._elementHorizontalSmallDecrease = this.GetTemplateChild("HorizontalSmallDecrease") as RepeatButton;
            this._elementHorizontalThumb = this.GetTemplateChild("HorizontalThumb") as Thumb;
            this._elementHorizontalThumbResizeInc = this.GetTemplateChild("HorizontalThumbResizeIncrease") as Thumb;
            this._elementHorizontalThumbResizeDec = this.GetTemplateChild("HorizontalThumbResizeDecrease") as Thumb;
            this._elementVerticalTemplate = this.GetTemplateChild("VerticalRoot") as FrameworkElement;
            this._elementVerticalLargeIncrease = this.GetTemplateChild("VerticalLargeIncrease") as RepeatButton;
            this._elementVerticalLargeDecrease = this.GetTemplateChild("VerticalLargeDecrease") as RepeatButton;
            this._elementVerticalSmallIncrease = this.GetTemplateChild("VerticalSmallIncrease") as RepeatButton;
            this._elementVerticalSmallDecrease = this.GetTemplateChild("VerticalSmallDecrease") as RepeatButton;
            this._elementVerticalThumb = this.GetTemplateChild("VerticalThumb") as Thumb;
            this._elementVerticalThumbResizeInc = this.GetTemplateChild("VerticalThumbResizeIncrease") as Thumb;
            this._elementVerticalThumbResizeDec = this.GetTemplateChild("VerticalThumbResizeDecrease") as Thumb;
            this._elementMinimizeVisualState = this.GetTemplateChild("MinimizeState") as VisualState;
            this._elementMaximizeVisualState = this.GetTemplateChild("MaximizeState") as VisualState;
            if (this._elementHorizontalThumb != null)
            {
                this._elementHorizontalThumb.DragStarted += (s, e) => this.OnThumbDragStarted();
                this._elementHorizontalThumb.DragDelta += (s, e) => this.OnThumbDragDelta(e);
                this._elementHorizontalThumb.DragCompleted += (s, e) => this.OnThumbDragCompleted();
            }
            this.AttachClickEventToRepeatButton(this._elementHorizontalLargeDecrease, new Action(this.LargeDecrement));
            this.AttachClickEventToRepeatButton(this._elementHorizontalLargeIncrease, new Action(this.LargeIncrement));
            this.AttachClickEventToRepeatButton(this._elementHorizontalSmallDecrease, new Action(this.SmallDecrement));
            this.AttachClickEventToRepeatButton(this._elementHorizontalSmallIncrease, new Action(this.SmallIncrement));
            this.AttachDragResizeEventToTumb(this._elementHorizontalThumbResizeDec);
            this.AttachDragResizeEventToTumb(this._elementHorizontalThumbResizeInc);
            if (this._elementVerticalThumb != null)
            {
                this._elementVerticalThumb.DragStarted += (s, e) => this.OnThumbDragStarted();
                this._elementVerticalThumb.DragDelta += (s, e) => this.OnThumbDragDelta(e);
                this._elementVerticalThumb.DragCompleted += (s, e) => this.OnThumbDragCompleted();
            }
            this.AttachClickEventToRepeatButton(this._elementVerticalLargeDecrease, new Action(this.LargeDecrement));
            this.AttachClickEventToRepeatButton(this._elementVerticalLargeIncrease, new Action(this.LargeIncrement));
            this.AttachClickEventToRepeatButton(this._elementVerticalSmallDecrease, new Action(this.SmallDecrement));
            this.AttachClickEventToRepeatButton(this._elementVerticalSmallIncrease, new Action(this.SmallIncrement));
            this.AttachDragResizeEventToTumb(this._elementVerticalThumbResizeDec);
            this.AttachDragResizeEventToTumb(this._elementVerticalThumbResizeInc);
            this.OnOrientationPropertyChanged(this.Orientation, this.Orientation);
            this.SetGrowAnimationParams(this._elementMinimizeVisualState, this.MinimizedSize);
            this.SetGrowAnimationParams(this._elementMaximizeVisualState, this.MaximizedSize);
            this.UpdateVisualState(false);
        }

        private void SetGrowAnimationParams(VisualState visualState, double size)
        {
            if (visualState == null)
                return;
            foreach (DoubleAnimation doubleAnimation in visualState.Storyboard.Children.OfType<DoubleAnimation>())
            {
                if (string.Equals(Storyboard.GetTargetName(doubleAnimation), "HorizontalRoot", StringComparison.OrdinalIgnoreCase) || string.Equals(Storyboard.GetTargetName(doubleAnimation), "VerticalRoot", StringComparison.OrdinalIgnoreCase))
                    doubleAnimation.To = new double?(size);
            }
        }

        private bool AttachClickEventToRepeatButton(RepeatButton button, Action action)
        {
            if (button == null)
                return false;
            button.Click += (s, e) => action();
            return true;
        }

        private bool AttachDragResizeEventToTumb(Thumb thumb)
        {
            if (thumb == null)
                return false;
            thumb.DragStarted += (s, e) => this.OnResizeDragStarted();
            thumb.DragDelta += (s, e) => this.OnResizeDragDelta(s, e);
            thumb.DragCompleted += (s, e) => this.OnResizeDragCompleted();
            return true;
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            this.UpdateVisualState();
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            if (!this._isAutoadjusting)
            {
                this._maximumAbsolute = newMaximum;
                this.OnViewportSizePropertyChanged(this.ViewportSize, this.ViewportSize);
            }
            double trackLength = this.GetTrackLength();
            base.OnMaximumChanged(oldMaximum, newMaximum);
            this.UpdateTrackLayout(trackLength);
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            double trackLength = this.GetTrackLength();
            base.OnMinimumChanged(oldMinimum, newMinimum);
            this.UpdateTrackLayout(trackLength);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this._isMouseOver = true;
            if ((this.Orientation != Orientation.Horizontal || this._elementHorizontalThumb == null || this._elementHorizontalThumb.IsDragging) && (this.Orientation != Orientation.Vertical || this._elementVerticalThumb == null || this._elementVerticalThumb.IsDragging))
                return;
            this.UpdateVisualState();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this._isMouseOver = false;
            if ((this.Orientation != Orientation.Horizontal || this._elementHorizontalThumb == null || this._elementHorizontalThumb.IsDragging) && (this.Orientation != Orientation.Vertical || this._elementVerticalThumb == null || this._elementVerticalThumb.IsDragging))
                return;
            this.UpdateVisualState();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled)
                return;
            e.Handled = true;
            this.CaptureMouse();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (e.Handled)
                return;
            e.Handled = true;
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            double trackLength = this.GetTrackLength();
            base.OnValueChanged(oldValue, newValue);
            this.UpdateTrackLayout(trackLength);
        }

        internal virtual void OnIsEnabledChanged()
        {
            if (!this.IsEnabled)
                this._isMouseOver = false;
            this.UpdateVisualState();
        }

        private double ConvertViewportSizeToDisplayUnits(double trackLength)
        {
            double num = this.Maximum - this.Minimum;
            return trackLength * this.ViewportSize / (this.ViewportSize + num);
        }

        internal double GetTrackLength()
        {
            if (this.Orientation == Orientation.Horizontal)
                return this.ActualWidth - this.BorderThickness.Width() - this.Padding.Width() - this.GetButtonLength(this._elementHorizontalSmallDecrease) - this.GetButtonLength(this._elementHorizontalSmallIncrease) - this.GetButtonLength(this._elementHorizontalThumbResizeDec) - this.GetButtonLength(this._elementHorizontalThumbResizeInc);
            return this.ActualHeight - this.BorderThickness.Height() - this.Padding.Height() - this.GetButtonLength(this._elementVerticalSmallDecrease) - this.GetButtonLength(this._elementVerticalSmallIncrease) - this.GetButtonLength(this._elementVerticalThumbResizeDec) - this.GetButtonLength(this._elementVerticalThumbResizeInc);
        }

        private void RaiseScrollEvent(ScrollEventType scrollEventType)
        {
            if (this.Scroll == null)
                return;
            this.Scroll(this, new ScrollEventArgs(scrollEventType, this.Value));
        }

        private double GetButtonLength(Control button)
        {
            if (button == null || button.Visibility == Visibility.Collapsed)
                return 0.0;
            if (this.Orientation == Orientation.Horizontal)
                return button.ActualWidth + button.Margin.Left + button.Margin.Right;
            return button.ActualHeight + button.Margin.Top + button.Margin.Right;
        }

        private void LargeDecrement()
        {
            double num = Math.Max(this.Value - this.LargeChange, this.Minimum);
            if (this.Value == num)
                return;
            this.Value = num;
            this.RaiseScrollEvent(ScrollEventType.LargeDecrement);
        }

        private void LargeIncrement()
        {
            double num = Math.Min(this.Value + this.LargeChange, this.Maximum);
            if (this.Value == num)
                return;
            this.Value = num;
            this.RaiseScrollEvent(ScrollEventType.LargeIncrement);
        }

        private void SmallDecrement()
        {
            double num = Math.Max(this.Value - this.SmallChange, this.Minimum);
            if (this.Value == num)
                return;
            this.Value = num;
            this.RaiseScrollEvent(ScrollEventType.SmallDecrement);
        }

        private void SmallIncrement()
        {
            double num = Math.Min(this.Value + this.SmallChange, this.Maximum);
            if (this.Value == num)
                return;
            this.Value = num;
            this.RaiseScrollEvent(ScrollEventType.SmallIncrement);
        }

        public void ScrollByAmount(ScrollAmount amount)
        {
            switch (amount)
            {
                case ScrollAmount.LargeDecrement:
                    this.LargeDecrement();
                    break;
                case ScrollAmount.SmallDecrement:
                    this.SmallDecrement();
                    break;
                case ScrollAmount.LargeIncrement:
                    this.LargeIncrement();
                    break;
                case ScrollAmount.SmallIncrement:
                    this.SmallIncrement();
                    break;
            }
        }

        internal void OnResizeDragCompleted()
        {
            this.IsResizing = false;
            if (this.ViewportSizeChangeCompleted == null)
                return;
            this.ViewportSizeChangeCompleted(this, new RoutedPropertyChangedEventArgs<double>(this._startDragViewportValue, this.ViewportSize));
        }

        internal void OnResizeDragDelta(object source, DragDeltaEventArgs e)
        {
            double num = this.Orientation == Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange;
            bool flag = false;
            if (source == this._elementHorizontalThumbResizeDec || source == this._elementVerticalThumbResizeDec)
            {
                flag = true;
                num *= -1.0;
            }
            double d = (this.Maximum + this.ViewportSize - this.Minimum) * num / this.GetTrackLength();
            if (double.IsNaN(d) || double.IsInfinity(d))
                return;
            this._currentDragViewportValue += d;
            this._currentDragViewportValue = Math.Max(0.0, this._currentDragViewportValue);
            double val2 = Math.Max(this.MinimumViewportSize, Math.Min(this.Maximum + this._currentDragViewportValue - this.Minimum, this._currentDragViewportValue));
            if (val2 == this.ViewportSize)
                return;
            if (flag)
            {
                if (this.Value - d < this.Minimum)
                    return;
                this.Value = Math.Max(this.Minimum, this.Value - d);
            }
            this.ViewportSize = Math.Min(this._maximumAbsolute - this.Value, val2);
        }

        internal void OnResizeDragStarted()
        {
            this._currentDragViewportValue = this._startDragViewportValue = this.ViewportSize;
            this.IsResizing = true;
        }

        internal void OnThumbDragCompleted()
        {
            this.StopTimer();
            this.UpdateDragValue(this, EventArgs.Empty);
            this.RaiseScrollEvent(ScrollEventType.EndScroll);
        }

        private void UpdateDragValue(object sender, EventArgs e)
        {
            if (double.IsNaN(this._pendingValue) || this._pendingValue == this.Value)
                return;
            this.Value = this._pendingValue;
            this.RaiseScrollEvent(ScrollEventType.ThumbTrack);
            this._pendingValue = double.NaN;
        }

        internal void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            double d = 0.0;
            double num = 1.0;
            if (this.Orientation == Orientation.Horizontal && this._elementHorizontalThumb != null)
                d = num * e.HorizontalChange / (this.GetTrackLength() - this._elementHorizontalThumb.ActualWidth) * (this.Maximum - this.Minimum);
            else if (this.Orientation == Orientation.Vertical && this._elementVerticalThumb != null)
                d = num * e.VerticalChange / (this.GetTrackLength() - this._elementVerticalThumb.ActualHeight) * (this.Maximum - this.Minimum);
            if (double.IsNaN(d) || double.IsInfinity(d))
                return;
            this._pendingValue = Math.Min(this.Maximum, Math.Max(this.Minimum, this.Value + d));
        }

        internal void OnThumbDragStarted()
        {
            this._dragValue = this.Value;
            this._pendingValue = double.NaN;
            this.StartTimer();
        }

        private void StartTimer()
        {
            if (this._timer == null)
                this._timer = new DispatcherTimer();
            else if (this._timer.IsEnabled)
                return;
            this._timer.Tick += new EventHandler(this.UpdateDragValue);
            this._timer.Interval = TimeSpan.FromMilliseconds(this.ValueDragInterval);
            this._timer.Start();
        }

        private void StopTimer()
        {
            if (this._timer == null)
                return;
            this._timer.Stop();
            this._timer.Tick -= new EventHandler(this.UpdateDragValue);
        }

        internal double UpdateThumbSize(double trackLength)
        {
            double num = double.NaN;
            bool flag = trackLength <= 0.0;
            if (trackLength > 0.0)
            {
                if (this.Orientation == Orientation.Horizontal && this._elementHorizontalThumb != null)
                {
                    if (this.Maximum - this.Minimum >= 0.0)
                        num = Math.Max(this._elementHorizontalThumb.MinWidth, this.ConvertViewportSizeToDisplayUnits(trackLength));
                    if (this.Maximum - this.Minimum < 0.0 || num > this.ActualWidth || trackLength <= this._elementHorizontalThumb.MinWidth)
                    {
                        flag = true;
                    }
                    else
                    {
                        this._elementHorizontalThumb.Visibility = Visibility.Visible;
                        this._elementHorizontalThumb.Width = num;
                        this._elementHorizontalThumb.IsEnabled = true;
                    }
                }
                else if (this.Orientation == Orientation.Vertical && this._elementVerticalThumb != null)
                {
                    if (this.Maximum - this.Minimum >= 0.0)
                        num = Math.Max(this._elementVerticalThumb.MinHeight, this.ConvertViewportSizeToDisplayUnits(trackLength));
                    if (this.Maximum - this.Minimum < 0.0 || num > this.ActualHeight || trackLength <= this._elementVerticalThumb.MinHeight)
                    {
                        flag = true;
                    }
                    else
                    {
                        this._elementVerticalThumb.Visibility = Visibility.Visible;
                        this._elementVerticalThumb.Height = num;
                    }
                }
            }
            if (flag)
            {
                if (this._elementHorizontalThumb != null)
                    this._elementHorizontalThumb.Visibility = Visibility.Collapsed;
                if (this._elementVerticalThumb != null)
                    this._elementVerticalThumb.Visibility = Visibility.Collapsed;
            }
            return num;
        }

        private void UpdateTrackLayout(double trackLength)
        {
            double num1 = (this.Value - this.Minimum) / (this.Maximum - this.Minimum);
            double num2 = this.UpdateThumbSize(trackLength);
            if (this.Orientation == Orientation.Horizontal && this._elementHorizontalLargeDecrease != null)
            {
                this._elementHorizontalLargeDecrease.Width = Math.Floor(Math.Max(0.0, num1 * (trackLength - num2)));
            }
            else
            {
                if (this.Orientation != Orientation.Vertical || this._elementVerticalLargeDecrease == null)
                    return;
                this._elementVerticalLargeDecrease.Height = Math.Floor(Math.Max(0.0, num1 * (trackLength - num2)));
            }
        }

        internal new void UpdateVisualState()
        {
            this.UpdateVisualState(true);
        }

        internal bool GoToState(bool useTransitions, string stateName)
        {
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        internal new virtual void UpdateVisualState(bool useTransitions)
        {
            if (!this.IsEnabled)
            {
                this.GoToState(useTransitions, "Disabled");
                this.UpdateVisualStateForSize(useTransitions, false);
            }
            else if (this._isMouseOver)
            {
                this.GoToState(useTransitions, "MouseOver");
                this.UpdateVisualStateForSize(useTransitions, true);
            }
            else
            {
                this.GoToState(useTransitions, "Normal");
                this.UpdateVisualStateForSize(useTransitions, false);
            }
        }

        private void UpdateVisualStateForSize(bool useTransitions, bool maximize)
        {
            if (this.IsAllwaysMaximized)
                this.GoToState(false, "MaximizeState");
            else
                this.GoToState(useTransitions, maximize ? "MaximizeState" : "MinimizeState");
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this._elementRootTemplate == null)
                return base.MeasureOverride(availableSize);
            Size size = base.MeasureOverride(availableSize);
            if (this.Orientation == Orientation.Horizontal)
            {
                size.Height = this.IsAllwaysMaximized ? this.MaximizedSize : this.MinimizedSize;
                size.Width = Math.Min(0.0, size.Width - 4.0);
            }
            else
            {
                size.Width = this.IsAllwaysMaximized ? this.MaximizedSize : this.MinimizedSize;
                size.Height = Math.Min(0.0, size.Height - 4.0);
            }
            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this._elementRootTemplate != null)
            {
                Rect finalRect = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);
                if (this.Orientation == Orientation.Horizontal)
                {
                    finalRect.Height = this._elementHorizontalTemplate.Height;
                    finalRect.X += 2.0;
                    if (this.VerticalAlignment != VerticalAlignment.Top)
                        finalRect.Y = finalSize.Height - finalRect.Height;
                }
                else
                {
                    finalRect.Width = this._elementVerticalTemplate.Width;
                    finalRect.Y += 2.0;
                    if (this.HorizontalAlignment != HorizontalAlignment.Left)
                        finalRect.X = finalSize.Width - finalRect.Width;
                }
                this._elementRootTemplate.Arrange(finalRect);
            }
            return finalSize;
        }
    }
}
