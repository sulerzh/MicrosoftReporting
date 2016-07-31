using Microsoft.Reporting.Common.Toolkit.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    [StyleTypedProperty(Property = "TitleStyle", StyleTargetType = typeof(Title))]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ContentPresenter))]
    public class Legend : HeaderedItemsControl, ISupportInitialize
    {
        public static readonly DependencyProperty TitleStyleProperty = DependencyProperty.Register("TitleStyle", typeof(Style), typeof(Legend), null);
        public static readonly DependencyProperty ContentVisibilityProperty = DependencyProperty.Register("ContentVisibility", typeof(Visibility), typeof(Legend), null);
        public static readonly DependencyProperty EqualItemHeightProperty = DependencyProperty.Register("EqualItemHeight", typeof(bool), typeof(Legend), new PropertyMetadata(false, (d, e) => ((Legend)d).OnEqualItemHeightPropertyChanged((bool)e.OldValue, (bool)e.NewValue)));
        public static readonly DependencyProperty EqualColumnWidthProperty = DependencyProperty.Register("EqualColumnWidth", typeof(bool), typeof(Legend), new PropertyMetadata(true, (d, e) => ((Legend)d).OnEqualColumnWidthPropertyChanged((bool)e.OldValue, (bool)e.NewValue)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Legend), new PropertyMetadata(Orientation.Vertical, (d, e) => ((Legend)d).OnOrientationPropertyChanged((Orientation)e.OldValue, (Orientation)e.NewValue)));
        public static readonly DependencyProperty ItemClickProperty = DependencyProperty.Register("ItemClick", typeof(ICommand), typeof(Legend), null);
        internal const string EqualItemHeightPropertyName = "EqualItemHeight";
        internal const string EqualColumnWidthPropertyName = "EqualColumnWidth";
        internal const string OrientationPropertyName = "Orientation";
        internal const string ItemClickPropertyName = "ItemClick";
        private LegendItem _currentSelectedItem;

        public Style TitleStyle
        {
            get
            {
                return this.GetValue(Legend.TitleStyleProperty) as Style;
            }
            set
            {
                this.SetValue(Legend.TitleStyleProperty, value);
            }
        }

        public Visibility ContentVisibility
        {
            get
            {
                return (Visibility)this.GetValue(Legend.ContentVisibilityProperty);
            }
            protected set
            {
                this.SetValue(Legend.ContentVisibilityProperty, value);
            }
        }

        [DefaultValue(false)]
        public bool EqualItemHeight
        {
            get
            {
                return (bool)this.GetValue(Legend.EqualItemHeightProperty);
            }
            set
            {
                this.SetValue(Legend.EqualItemHeightProperty, value);
            }
        }

        [DefaultValue(true)]
        public bool EqualColumnWidth
        {
            get
            {
                return (bool)this.GetValue(Legend.EqualColumnWidthProperty);
            }
            set
            {
                this.SetValue(Legend.EqualColumnWidthProperty, value);
            }
        }

        [DefaultValue(Orientation.Vertical)]
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(Legend.OrientationProperty);
            }
            set
            {
                this.SetValue(Legend.OrientationProperty, value);
            }
        }

        public ICommand ItemClick
        {
            get
            {
                return (ICommand)this.GetValue(Legend.ItemClickProperty);
            }
            set
            {
                this.SetValue(Legend.ItemClickProperty, value);
            }
        }

        internal bool Initializing { get; set; }

        internal LegendItem SelectedItem
        {
            get
            {
                foreach (LegendItem legendItem in this.Items)
                {
                    if (legendItem.IsSelected)
                        return legendItem;
                }
                return null;
            }
        }

        public event SelectionChangedEventHandler SelectionChanged;

        public Legend()
        {
            this.DefaultStyleKey = typeof(Legend);
            this.SetBinding(UIElement.VisibilityProperty, new Binding("ContentVisibility")
            {
                Source = (this)
            });
        }

        private void OnEqualItemHeightPropertyChanged(bool oldValue, bool newValue)
        {
            this.UpdateItemHeights();
        }

        private void OnEqualColumnWidthPropertyChanged(bool oldValue, bool newValue)
        {
            this.UpdateColumnWidths();
        }

        private void OnOrientationPropertyChanged(Orientation oldValue, Orientation newValue)
        {
            this.UpdateOrientation();
            this.UpdateItemMargins();
        }

        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);
            this.UpdateContentVisibility();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            foreach (LegendItem legendItem in this.Items)
                legendItem.Owner = this;
            this._currentSelectedItem = null;
            base.OnItemsChanged(e);
            this.UpdateContentVisibility();
            this.UpdateOrientation();
            this.UpdateColumnWidths();
            this.UpdateItemMargins();
            this.UpdateItemHeights();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.UpdateItemMargins();
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            this.UpdateOrientation();
            this.UpdateColumnWidths();
            this.UpdateItemHeights();
            return size;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (!this.Items.OfType<LegendItem>().Any<LegendItem>(item => !item.IsSelected))
                return;
            this.FireItemClick(this.SelectedItem);
        }

        public void FireItemClick(LegendItem item)
        {
            if (item != null && this.ItemClick != null && this.ItemClick.CanExecute(item))
                this.ItemClick.Execute(item);
            this.FireSelectionChanged(item);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (this.SelectedItem != null || this.Items.Count <= 0)
                return;
            ((UIElement)this.Items[0]).Focus();
        }

        public void ClearSelection()
        {
            this._currentSelectedItem = null;
            foreach (LegendItem legendItem in this.Items)
                legendItem.IsSelected = false;
        }

        private void OnSelectionChanged(SelectionChangedEventArgs args)
        {
            SelectionChangedEventHandler changedEventHandler = this.SelectionChanged;
            if (changedEventHandler == null)
                return;
            changedEventHandler(this, args);
        }

        private void FireSelectionChanged(LegendItem selectedItem = null)
        {
            if (selectedItem == null)
                selectedItem = this.SelectedItem;
            if (selectedItem == this._currentSelectedItem)
                return;
            this.OnSelectionChanged(new SelectionChangedEventArgs(null, new object[1]
            {
         this._currentSelectedItem
            }, new object[1]
            {
         selectedItem
            }));
            this._currentSelectedItem = selectedItem;
        }

        internal void ChangeViewPoint(bool isforward)
        {
            ScrollViewer scrollHost = this.GetScrollHost();
            if (scrollHost == null)
                return;
            if (isforward)
            {
                if (this.Orientation == Orientation.Vertical)
                    scrollHost.PageDown();
                else
                    scrollHost.PageRight();
            }
            else if (this.Orientation == Orientation.Vertical)
                scrollHost.PageUp();
            else
                scrollHost.PageLeft();
        }

        internal LegendItem GetNextItem(LegendItem currentItem, bool isForward)
        {
            int num = this.Items.IndexOf(currentItem);
            int index = !isForward ? num - 1 : num + 1;
            if (index < 0)
                index = 0;
            else if (index > this.Items.Count - 1)
                index = this.Items.Count - 1;
            return (LegendItem)this.Items[index];
        }

        internal bool SetFocusToItem(LegendItem item)
        {
            bool flag = false;
            if (item != null)
            {
                if (!item.IsInLegendVisibleArea(this))
                {
                    ScrollViewer scrollHost = this.GetScrollHost();
                    if (scrollHost != null)
                        scrollHost.ScrollIntoView(item, 0.0, 0.0, TimeSpan.Zero.Duration());
                }
                flag = item.Focus();
            }
            return flag;
        }

        private void UpdateOrientation()
        {
            if (this.Initializing || this.Items.Count <= 0)
                return;
            StackPanel stackPanel = VisualTreeHelper.GetParent((DependencyObject)this.Items[0]) as StackPanel;
            if (stackPanel != null && stackPanel.Orientation != this.Orientation)
            {
                stackPanel.Orientation = this.Orientation;
                ScrollViewer scrollHost = this.GetScrollHost();
                if (scrollHost != null)
                {
                    scrollHost.HorizontalScrollBarVisibility = this.Orientation == Orientation.Vertical ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
                    scrollHost.VerticalScrollBarVisibility = this.Orientation == Orientation.Horizontal ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
                }
            }
            foreach (object obj in this.Items)
            {
                LegendItem legendItem = obj as LegendItem;
                if (legendItem != null && legendItem.Title != null)
                    legendItem.Title.TextTrimming = this.Orientation == Orientation.Vertical ? TextTrimming.WordEllipsis : TextTrimming.None;
            }
        }

        private void UpdateItemMargins()
        {
            if (this.Initializing)
                return;
            if (this.Orientation == Orientation.Horizontal)
            {
                foreach (object obj in this.Items)
                {
                    LegendItem legendItem = obj as LegendItem;
                    if (legendItem != null)
                    {
                        if (this.Items.IndexOf(obj) != this.Items.Count - 1)
                            legendItem.ExtendMargin();
                        else
                            legendItem.RestoreOriginalMargin();
                    }
                }
            }
            else
            {
                foreach (object obj in this.Items)
                {
                    LegendItem legendItem = obj as LegendItem;
                    if (legendItem != null)
                        legendItem.RestoreOriginalMargin();
                }
            }
        }

        private void UpdateItemHeights()
        {
            if (this.Initializing)
                return;
            if (this.EqualItemHeight)
            {
                double val2 = 0.0;
                foreach (object obj in this.Items)
                {
                    LegendItem legendItem = obj as LegendItem;
                    if (legendItem != null)
                        val2 = Math.Max(legendItem.ActualHeight, val2);
                }
                if (val2 <= 0.0)
                    return;
                foreach (object obj in this.Items)
                {
                    LegendItem legendItem = obj as LegendItem;
                    if (legendItem != null)
                        legendItem.MinHeight = val2;
                }
            }
            else
            {
                foreach (object obj in this.Items)
                {
                    LegendItem legendItem = obj as LegendItem;
                    if (legendItem != null)
                        legendItem.MinHeight = 0.0;
                }
            }
        }

        private void UpdateColumnWidths()
        {
            if (this.Initializing)
                return;
            FrameworkElement[][] cells = this.GetCells();
            int maxumumColumnCount = this.DetermineMaxumumColumnCount(cells);
            if (this.EqualColumnWidth)
            {
                for (int index1 = 0; index1 < maxumumColumnCount; ++index1)
                {
                    double val2 = 0.0;
                    for (int index2 = 0; index2 < cells.Length; ++index2)
                    {
                        if (cells[index2].Length > index1 && cells[index2][index1] != null)
                        {
                            LegendSymbol legendSymbol = cells[index2][index1] as LegendSymbol;
                            if (legendSymbol != null)
                                val2 = Math.Max(legendSymbol.GetDesiredSymbolWidth(), val2);
                            else if (cells[index2][index1] != null)
                                val2 = !double.IsNaN(cells[index2][index1].Width) ? Math.Max(Math.Max(cells[index2][index1].Width, cells[index2][index1].ActualWidth), val2) : Math.Max(cells[index2][index1].ActualWidth, val2);
                        }
                    }
                    if (val2 > 0.0)
                    {
                        for (int index2 = 0; index2 < cells.Length; ++index2)
                        {
                            if (cells[index2].Length > index1)
                            {
                                LegendSymbol legendSymbol = cells[index2][index1] as LegendSymbol;
                                if (legendSymbol != null)
                                    legendSymbol.ActualSymbolWidth = val2;
                                else if (cells[index2][index1] != null)
                                    cells[index2][index1].MinWidth = val2;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int index1 = 0; index1 < maxumumColumnCount; ++index1)
                {
                    for (int index2 = 0; index2 < cells.Length; ++index2)
                    {
                        if (cells[index2].Length > index1 && cells[index2][index1] != null)
                        {
                            LegendSymbol legendSymbol = cells[index2][index1] as LegendSymbol;
                            if (legendSymbol != null)
                                legendSymbol.ActualSymbolWidth = legendSymbol.GetDesiredSymbolWidth();
                            else if (cells[index2][index1] != null)
                                cells[index2][index1].MinWidth = 0.0;
                        }
                    }
                }
            }
        }

        private int DetermineMaxumumColumnCount(FrameworkElement[][] cells)
        {
            int val1 = 0;
            foreach (FrameworkElement[] cell in cells)
                val1 = Math.Max(val1, cell.Length);
            return val1;
        }

        private FrameworkElement[][] GetCells()
        {
            List<FrameworkElement[]> frameworkElementArrayList = new List<FrameworkElement[]>();
            foreach (object obj in this.Items)
            {
                LegendItem legendItem = obj as LegendItem;
                if (legendItem != null)
                    frameworkElementArrayList.Add(legendItem.GetCells());
            }
            return frameworkElementArrayList.ToArray();
        }

        private void UpdateContentVisibility()
        {
            this.ContentVisibility = this.Header != null || this.Items.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new Legend.LegendAutomationPeer(this);
        }

        public new void BeginInit()
        {
            this.Initializing = true;
        }

        public new void EndInit()
        {
            this.Initializing = false;
            this.UpdateContentVisibility();
            this.UpdateOrientation();
            this.UpdateColumnWidths();
            this.UpdateItemMargins();
            this.UpdateItemHeights();
        }

        internal static class Defaults
        {
            internal const bool EqualItemHeight = false;
            internal const bool EqualColumnWidth = true;
        }

        protected sealed class LegendAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider
        {
            private Legend Legend
            {
                get
                {
                    return (Legend)this.Owner;
                }
            }

            bool ISelectionProvider.CanSelectMultiple
            {
                get
                {
                    return true;
                }
            }

            bool ISelectionProvider.IsSelectionRequired
            {
                get
                {
                    return false;
                }
            }

            public LegendAutomationPeer(Legend owner)
              : base(owner)
            {
            }

            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.List;
            }

            protected override string GetClassNameCore()
            {
                return Properties.Resources.Automation_LegendClassName;
            }

            protected override string GetNameCore()
            {
                string str = base.GetNameCore();
                if (string.IsNullOrEmpty(str))
                    str = this.Legend.Header as string;
                if (string.IsNullOrEmpty(str))
                    str = this.Legend.Name;
                if (string.IsNullOrEmpty(str))
                    str = this.Legend.GetType().Name;
                return str;
            }

            public override object GetPattern(PatternInterface patternInterface)
            {
                if (patternInterface == PatternInterface.Selection)
                    return this;
                return null;
            }

            protected override Rect GetBoundingRectangleCore()
            {
                return base.GetBoundingRectangleCore();
            }

            protected override List<AutomationPeer> GetChildrenCore()
            {
                return this.Legend.Items.OfType<LegendItem>().Select<LegendItem, AutomationPeer>(legendTtem => UIElementAutomationPeer.CreatePeerForElement(legendTtem)).ToList<AutomationPeer>();
            }

            IRawElementProviderSimple[] ISelectionProvider.GetSelection()
            {
                return this.Legend.Items.OfType<LegendItem>().Where<LegendItem>(li => li.IsSelected).Select<LegendItem, IRawElementProviderSimple>(legendTtem => this.ProviderFromPeer(UIElementAutomationPeer.CreatePeerForElement(legendTtem))).ToArray<IRawElementProviderSimple>();
            }
        }
    }
}
