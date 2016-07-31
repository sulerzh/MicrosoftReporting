using Microsoft.Reporting.Windows.Common.Internal.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Unfocused")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
    [ContentProperty("Items")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Focused")]
    public class LegendItem : ItemsControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(LegendItem), new PropertyMetadata(null));
        public static readonly DependencyProperty SymbolContentProperty = DependencyProperty.Register("SymbolContent", typeof(object), typeof(LegendItem), new PropertyMetadata(null));
        public static readonly DependencyProperty SymbolWidthProperty = DependencyProperty.Register("SymbolWidth", typeof(double?), typeof(LegendItem), new PropertyMetadata(null));
        public static readonly DependencyProperty SymbolHeightProperty = DependencyProperty.Register("SymbolHeight", typeof(double?), typeof(LegendItem), new PropertyMetadata(null));
        public static readonly DependencyProperty SymbolBorderBrushProperty = DependencyProperty.Register("SymbolBorderBrush", typeof(Brush), typeof(LegendItem), new PropertyMetadata(null));
        public static readonly DependencyProperty SymbolBorderThicknessProperty = DependencyProperty.Register("SymbolBorderThickness", typeof(double), typeof(LegendItem), new PropertyMetadata(1.0));
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(LegendItem), new PropertyMetadata(null));
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(LegendItem), new PropertyMetadata(3.0));
        public static readonly DependencyProperty MarkerProperty = DependencyProperty.Register("Marker", typeof(MarkerType), typeof(LegendItem), new PropertyMetadata(MarkerType.None));
        public static readonly DependencyProperty MarkerSizeProperty = DependencyProperty.Register("MarkerSize", typeof(double), typeof(LegendItem), new PropertyMetadata(10.0));
        public static readonly DependencyProperty MarkerFillProperty = DependencyProperty.Register("MarkerFill", typeof(Brush), typeof(LegendItem), new PropertyMetadata(LegendSymbol.Defaults.MarkerFill));
        public static readonly DependencyProperty MarkerStrokeProperty = DependencyProperty.Register("MarkerStroke", typeof(Brush), typeof(LegendItem), new PropertyMetadata(LegendSymbol.Defaults.MarkerStroke));
        public static readonly DependencyProperty MarkerStrokeThicknessProperty = DependencyProperty.Register("MarkerStrokeThickness", typeof(double), typeof(LegendItem), new PropertyMetadata(1.0));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(LegendItem), new PropertyMetadata(true, new PropertyChangedCallback(LegendItem.OnIsSelectedPropertyChanged)));
        public new static readonly DependencyProperty IsFocusedProperty = DependencyProperty.Register("IsFocused", typeof(bool), typeof(LegendItem), new PropertyMetadata(false, new PropertyChangedCallback(LegendItem.OnIsFocusedPropertyChanged)));
        public static readonly DependencyProperty UnselectedOpacityProperty = DependencyProperty.Register("UnselectedOpacity", typeof(double), typeof(LegendItem), new PropertyMetadata(0.6, new PropertyChangedCallback(LegendItem.OnUnselectedOpacityPropertyChanged)));
        public static readonly DependencyProperty UnselectedEffectProperty = DependencyProperty.Register("UnselectedEffect", typeof(Effect), typeof(LegendItem), new PropertyMetadata(new GloomEffect() { BaseSaturation = 0.3 }, new PropertyChangedCallback(LegendItem.OnUnselectedEffectPropertyChanged)));
        public static readonly DependencyProperty ActualEffectProperty = DependencyProperty.Register("ActualEffect", typeof(Effect), typeof(LegendItem), new PropertyMetadata(null, new PropertyChangedCallback(LegendItem.OnActualEffectPropertyChanged)));
        public static readonly DependencyProperty ActualOpacityProperty = DependencyProperty.Register("ActualOpacity", typeof(double), typeof(LegendItem), new PropertyMetadata(1.0, new PropertyChangedCallback(LegendItem.OnActualOpacityPropertyChanged)));
        public static readonly DependencyProperty OpacityListenerProperty = DependencyProperty.RegisterAttached("LegendOpacityListener", typeof(object), typeof(FrameworkElement), new PropertyMetadata(new PropertyChangedCallback(LegendItem.OnOpacityChanged)));
        public static readonly DependencyProperty EffectListenerProperty = DependencyProperty.RegisterAttached("LegendEffectListener", typeof(object), typeof(FrameworkElement), new PropertyMetadata(new PropertyChangedCallback(LegendItem.OnEffectChanged)));
        public static readonly DependencyProperty ActualCursorProperty = DependencyProperty.Register("ActualCursor", typeof(Cursor), typeof(LegendItem), new PropertyMetadata(null, new PropertyChangedCallback(LegendItem.OnActualCursorPropertyChanged)));
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(object), typeof(LegendItem), new PropertyMetadata(null));
        internal const string OpacityPropertyName = "Opacity";
        internal const string EffectPropertyName = "Effect";
        internal const string FillPropertyName = "Fill";
        internal const string SymbolContentPropertyName = "SymbolContent";
        internal const string SymbolWidthPropertyName = "SymbolWidth";
        internal const string SymbolHeightPropertyName = "SymbolHeight";
        internal const string SymbolBorderBrushPropertyName = "SymbolBorderBrush";
        internal const string SymbolBorderThicknessPropertyName = "SymbolBorderThickness";
        internal const string StrokePropertyName = "Stroke";
        internal const string StrokeThicknessPropertyName = "StrokeThickness";
        internal const string MarkerPropertyName = "Marker";
        internal const string MarkerSizePropertyName = "MarkerSize";
        internal const string MarkerFillPropertyName = "MarkerFill";
        internal const string MarkerStrokePropertyName = "MarkerStroke";
        internal const string MarkerStrokeThicknessPropertyName = "MarkerStrokeThickness";
        internal const string IsSelectedPropertyName = "IsSelected";
        internal const string IsFocusedPropertyName = "IsFocused";
        internal const string UnselectedOpacityPropertyName = "UnselectedOpacity";
        internal const string UnselectedEffectPropertyName = "UnselectedEffect";
        internal const string ActualEffectPropertyName = "ActualEffect";
        internal const string ActualOpacityPropertyName = "ActualOpacity";
        internal const string ActualCursorPropertyName = "ActualCursor";
        internal const string LabelPropertyName = "Label";
        private bool _marginExtended;
        private Thickness _originalMargin;

        public Legend Owner { get; set; }

        public Brush Fill
        {
            get
            {
                return this.GetValue(LegendItem.FillProperty) as Brush;
            }
            set
            {
                this.SetValue(LegendItem.FillProperty, value);
            }
        }

        public object SymbolContent
        {
            get
            {
                return this.GetValue(LegendItem.SymbolContentProperty);
            }
            set
            {
                this.SetValue(LegendItem.SymbolContentProperty, value);
            }
        }

        public double? SymbolWidth
        {
            get
            {
                return (double?)this.GetValue(LegendItem.SymbolWidthProperty);
            }
            set
            {
                this.SetValue(LegendItem.SymbolWidthProperty, value);
            }
        }

        public double? SymbolHeight
        {
            get
            {
                return (double?)this.GetValue(LegendItem.SymbolHeightProperty);
            }
            set
            {
                this.SetValue(LegendItem.SymbolHeightProperty, value);
            }
        }

        public Brush SymbolBorderBrush
        {
            get
            {
                return this.GetValue(LegendItem.SymbolBorderBrushProperty) as Brush;
            }
            set
            {
                this.SetValue(LegendItem.SymbolBorderBrushProperty, value);
            }
        }

        [DefaultValue(1.0)]
        public double SymbolBorderThickness
        {
            get
            {
                return (double)this.GetValue(LegendItem.SymbolBorderThicknessProperty);
            }
            set
            {
                this.SetValue(LegendItem.SymbolBorderThicknessProperty, value);
            }
        }

        public Brush Stroke
        {
            get
            {
                return this.GetValue(LegendItem.StrokeProperty) as Brush;
            }
            set
            {
                this.SetValue(LegendItem.StrokeProperty, value);
            }
        }

        [DefaultValue(3.0)]
        public double StrokeThickness
        {
            get
            {
                return (double)this.GetValue(LegendItem.StrokeThicknessProperty);
            }
            set
            {
                this.SetValue(LegendItem.StrokeThicknessProperty, value);
            }
        }

        [DefaultValue(MarkerType.None)]
        public MarkerType Marker
        {
            get
            {
                return (MarkerType)this.GetValue(LegendItem.MarkerProperty);
            }
            set
            {
                this.SetValue(LegendItem.MarkerProperty, value);
            }
        }

        [DefaultValue(10.0)]
        public double MarkerSize
        {
            get
            {
                return (double)this.GetValue(LegendItem.MarkerSizeProperty);
            }
            set
            {
                this.SetValue(LegendItem.MarkerSizeProperty, value);
            }
        }

        public Brush MarkerFill
        {
            get
            {
                return this.GetValue(LegendItem.MarkerFillProperty) as Brush;
            }
            set
            {
                this.SetValue(LegendItem.MarkerFillProperty, value);
            }
        }

        public Brush MarkerStroke
        {
            get
            {
                return this.GetValue(LegendItem.MarkerStrokeProperty) as Brush;
            }
            set
            {
                this.SetValue(LegendItem.MarkerStrokeProperty, value);
            }
        }

        [DefaultValue(1.0)]
        public double MarkerStrokeThickness
        {
            get
            {
                return (double)this.GetValue(LegendItem.MarkerStrokeThicknessProperty);
            }
            set
            {
                this.SetValue(LegendItem.MarkerStrokeThicknessProperty, value);
            }
        }

        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(LegendItem.IsSelectedProperty);
            }
            set
            {
                this.SetValue(LegendItem.IsSelectedProperty, value);
            }
        }

        public new bool IsFocused
        {
            get
            {
                return (bool)this.GetValue(LegendItem.IsFocusedProperty);
            }
            private set
            {
                this.SetValue(LegendItem.IsFocusedProperty, value);
            }
        }

        public double UnselectedOpacity
        {
            get
            {
                return (double)this.GetValue(LegendItem.UnselectedOpacityProperty);
            }
            set
            {
                this.SetValue(LegendItem.UnselectedOpacityProperty, value);
            }
        }

        public Effect UnselectedEffect
        {
            get
            {
                return (Effect)this.GetValue(LegendItem.UnselectedEffectProperty);
            }
            set
            {
                this.SetValue(LegendItem.UnselectedEffectProperty, value);
            }
        }

        public Effect ActualEffect
        {
            get
            {
                return (Effect)this.GetValue(LegendItem.ActualEffectProperty);
            }
            set
            {
                this.SetValue(LegendItem.ActualEffectProperty, value);
            }
        }

        public double ActualOpacity
        {
            get
            {
                return (double)this.GetValue(LegendItem.ActualOpacityProperty);
            }
            set
            {
                this.SetValue(LegendItem.ActualOpacityProperty, value);
            }
        }

        public Cursor ActualCursor
        {
            get
            {
                return (Cursor)this.GetValue(LegendItem.ActualCursorProperty);
            }
            set
            {
                this.SetValue(LegendItem.ActualCursorProperty, value);
            }
        }

        public object Label
        {
            get
            {
                return this.GetValue(LegendItem.LabelProperty);
            }
            set
            {
                this.SetValue(LegendItem.LabelProperty, value);
            }
        }

        internal LegendSymbol Symbol { get; set; }

        internal TextBlock Title { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public LegendItem()
        {
            this.DefaultStyleKey = typeof(LegendItem);
        }

        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.OldValue == (bool)e.NewValue)
                return;
            ((LegendItem)d).OnIsSelectedPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsSelectedPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("IsSelected");
        }

        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LegendItem)d).OnIsFocusedPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsFocusedPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("IsFocused");
        }

        private static void OnUnselectedOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LegendItem)d).OnUnselectedOpacityPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnUnselectedOpacityPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("UnselectedOpacity");
        }

        private static void OnUnselectedEffectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LegendItem)d).OnUnselectedEffectPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnUnselectedEffectPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("UnselectedEffect");
        }

        private static void OnActualEffectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LegendItem)d).OnActualEffectPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualEffectPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ActualEffect");
        }

        private static void OnActualOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LegendItem)d).OnActualOpacityPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualOpacityPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ActualOpacity");
        }

        private static void OnOpacityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (!(o is LegendItem))
                return;
            ((LegendItem)o).OnPropertyChanged("Opacity");
        }

        private static void OnEffectChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (!(o is LegendItem))
                return;
            ((LegendItem)o).OnPropertyChanged("Effect");
        }

        private static void OnActualCursorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LegendItem)d).OnActualCursorPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualCursorPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ActualCursor");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Symbol = this.GetTemplateChild("Symbol") as LegendSymbol;
            this.Title = this.GetTemplateChild("Title") as TextBlock;
            this.UpdateActualOpacityAndEffect();
            Binding binding1 = new Binding("Opacity") { Source = (this) };
            this.SetBinding(LegendItem.OpacityListenerProperty, binding1);
            Binding binding2 = new Binding("Effect") { Source = (this) };
            this.SetBinding(LegendItem.EffectListenerProperty, binding2);
            if (this.Title == null || this.Owner == null)
                return;
            this.Title.TextTrimming = this.Owner.Orientation == Orientation.Vertical ? TextTrimming.WordEllipsis : TextTrimming.None;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            Legend owner = this.Owner;
            Cursor cursor = null;
            if (owner != null && owner.ItemClick != null && owner.ItemClick.CanExecute(this))
                cursor = Cursors.Hand;
            this.ActualCursor = cursor;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.IsFocused = true;
            this.ChangeVisualState(true);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.IsFocused = false;
            this.ChangeVisualState(true);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled || !this.IsEnabled || !this.IsTabStop)
                return;
            this.Dispatcher.BeginInvoke((Action)(() => this.Focus()));
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (e.Handled || !this.IsEnabled)
                return;
            Legend owner = this.Owner;
            if (owner == null)
                return;
            owner.FireItemClick(this);
            e.Handled = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!this.IsEnabled)
                return;
            e.Handled = this.ProcessKeysUp(e.Key);
            if (e.Handled)
                return;
            base.OnKeyUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!this.IsEnabled)
                return;
            e.Handled = this.ProcessKeysDown(e.Key);
            if (e.Handled)
                return;
            base.OnKeyDown(e);
        }

        private bool ProcessKeysDown(Key key)
        {
            bool flag = false;
            Legend owner = this.Owner;
            if (owner == null)
                return flag;
            switch (key)
            {
                case Key.Prior:
                    owner.ChangeViewPoint(false);
                    break;
                case Key.Next:
                    owner.ChangeViewPoint(true);
                    break;
                case Key.Left:
                    flag = this.TrySetFocusOnNextItem(owner, false);
                    break;
                case Key.Up:
                    flag = this.TrySetFocusOnNextItem(owner, false);
                    break;
                case Key.Right:
                    flag = this.TrySetFocusOnNextItem(owner, true);
                    break;
                case Key.Down:
                    flag = this.TrySetFocusOnNextItem(owner, true);
                    break;
            }
            return flag;
        }

        private bool ProcessKeysUp(Key key)
        {
            bool flag = false;
            Legend legend = this.Owner;
            if (legend == null)
                return flag;
            switch (key)
            {
                case Key.Return:
                case Key.Space:
                    legend.FireItemClick(this);
                    break;
                case Key.Prior:
                    LegendItem legendItem1 = legend.Items.OfType<LegendItem>().FirstOrDefault<LegendItem>(item => item.IsInLegendVisibleArea(legend));
                    flag = legend.SetFocusToItem(legendItem1);
                    break;
                case Key.Next:
                    if (!this.IsInLegendVisibleArea(legend))
                    {
                        LegendItem legendItem2 = legend.Items.OfType<LegendItem>().FirstOrDefault<LegendItem>(item => item.IsInLegendVisibleArea(legend));
                        flag = legend.SetFocusToItem(legendItem2);
                        break;
                    }
                    LegendItem legendItem3 = (LegendItem)legend.Items[legend.Items.Count - 1];
                    legend.SetFocusToItem(legendItem3);
                    break;
                case Key.End:
                    LegendItem legendItem4 = (LegendItem)legend.Items[legend.Items.Count - 1];
                    flag = legend.SetFocusToItem(legendItem4);
                    break;
                case Key.Home:
                    LegendItem legendItem5 = (LegendItem)legend.Items[0];
                    flag = legend.SetFocusToItem(legendItem5);
                    break;
            }
            return flag;
        }

        private bool TrySetFocusOnNextItem(Legend legend, bool isForward)
        {
            LegendItem nextItem = legend.GetNextItem(this, isForward);
            return legend.SetFocusToItem(nextItem);
        }

        internal bool IsInLegendVisibleArea(Legend legend)
        {
            Point point;
            try
            {
                point = this.TransformToVisual(legend).Transform(new Point(0.0, 0.0));
            }
            catch
            {
                point = new Point(double.NegativeInfinity, double.NegativeInfinity);
            }
            if (point.X >= 0.0 && point.Y >= 0.0 && point.X + this.ActualWidth / 2.0 <= legend.ActualWidth)
                return point.Y + this.ActualHeight / 2.0 <= legend.ActualHeight;
            return false;
        }

        internal void ExtendMargin()
        {
            if (this._marginExtended)
                return;
            Thickness margin = this.Margin;
            this._originalMargin = this.Margin;
            this.Margin = new Thickness(this._originalMargin.Left, this._originalMargin.Top, this._originalMargin.Right + 20.0, this._originalMargin.Bottom);
            this.Margin = new Thickness(0.0, 0.0, 20.0, 0.0);
        }

        internal void RestoreOriginalMargin()
        {
            if (!this._marginExtended)
                return;
            this.Margin = this._originalMargin;
        }

        internal FrameworkElement[] GetCells()
        {
            List<FrameworkElement> frameworkElementList = new List<FrameworkElement>();
            if (this.Symbol != null && this.Symbol.GetDesiredSymbolWidth() > 0.0)
                frameworkElementList.Add(this.Symbol);
            if (this.Label != null)
                frameworkElementList.Add(this.Title);
            foreach (object obj in this.Items)
            {
                if (obj is FrameworkElement)
                    frameworkElementList.Add((FrameworkElement)obj);
            }
            return frameworkElementList.ToArray();
        }

        private void UpdateActualOpacityAndEffect()
        {
            this.ActualOpacity = this.IsSelected ? this.Opacity : this.UnselectedOpacity;
            this.ActualEffect = this.IsSelected ? this.Effect : this.UnselectedEffect;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new LegendItem.LegendItemAutomationPeer(this);
        }

        internal bool GoToState(string stateName, bool useTransitions)
        {
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        internal new virtual void ChangeVisualState(bool useTransitions)
        {
            if (!this.IsEnabled)
                this.GoToState("Disabled", useTransitions);
            else
                this.GoToState("Normal", useTransitions);
            if (this.IsFocused && this.IsEnabled)
                this.GoToState("Focused", useTransitions);
            else
                this.GoToState("Unfocused", useTransitions);
        }

        internal void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "Opacity":
                case "Effect":
                case "IsSelected":
                case "UnselectedEffect":
                case "UnselectedOpacity":
                    this.UpdateActualOpacityAndEffect();
                    break;
            }
            if (this.PropertyChanged == null)
                return;
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected sealed class LegendItemAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
        {
            private LegendItem LegendItem
            {
                get
                {
                    return (LegendItem)this.Owner;
                }
            }

            private Legend Legend
            {
                get
                {
                    return this.LegendItem.Owner;
                }
            }

            public LegendItemAutomationPeer(LegendItem owner)
              : base(owner)
            {
            }

            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.ListItem;
            }

            protected override string GetClassNameCore()
            {
                return Properties.Resources.Automation_LegendItemClassName;
            }

            protected override string GetAutomationIdCore()
            {
                string str = base.GetAutomationIdCore();
                if (string.IsNullOrEmpty(str))
                {
                    str = this.GetName();
                    if (this.Legend != null)
                    {
                        int num = this.Legend.Items.IndexOf(this.LegendItem);
                        if (num != -1)
                            str = "LegendItem" + num.ToString(CultureInfo.InvariantCulture);
                    }
                }
                return str;
            }

            protected override string GetNameCore()
            {
                string str = base.GetNameCore();
                if (string.IsNullOrEmpty(str))
                    str = this.LegendItem.Label as string;
                if (string.IsNullOrEmpty(str))
                    str = this.LegendItem.Name;
                if (string.IsNullOrEmpty(str))
                    str = this.LegendItem.GetType().Name;
                return str;
            }

            public override object GetPattern(PatternInterface patternInterface)
            {
                if (patternInterface == PatternInterface.Invoke)
                    return this;
                return null;
            }

            public void Invoke()
            {
                if (this.Legend == null)
                    return;
                this.Legend.FireItemClick(this.LegendItem);
            }
        }
    }
}
