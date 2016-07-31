using Microsoft.Reporting.Windows.Chart.Internal.Properties;
using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    [StyleTypedProperty(Property = "ToolTipStyle", StyleTargetType = typeof(ToolTip))]
    [StyleTypedProperty(Property = "LabelStyle", StyleTargetType = typeof(LabelControl))]
    [StyleTypedProperty(Property = "MarkerStyle", StyleTargetType = typeof(MarkerControl))]
    public abstract class DataPoint : FrameworkElement, IUpdatable, IUpdateSessionProvider, IAppearanceProvider, INotifyValueChanged, INotifyPropertyChanged
    {
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnFillChanged)));
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnStrokeChanged)));
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(DataPoint), new PropertyMetadata(1.0, new PropertyChangedCallback(DataPoint.OnStrokeThicknessChanged)));
        public static readonly DependencyProperty StrokeDashTypeProperty = DependencyProperty.Register("StrokeDashType", typeof(StrokeDashType), typeof(DataPoint), new PropertyMetadata(StrokeDashType.Solid, new PropertyChangedCallback(DataPoint.OnStrokeDashTypeChanged)));
        public static readonly DependencyProperty IsEmptyProperty = DependencyProperty.Register("IsEmpty", typeof(bool), typeof(DataPoint), new PropertyMetadata(false, new PropertyChangedCallback(DataPoint.OnIsEmptyChanged)));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(DataPoint), new PropertyMetadata(false, new PropertyChangedCallback(DataPoint.OnIsSelectedPropertyChanged)));
        public static readonly DependencyProperty IsDimmedProperty = DependencyProperty.Register("IsDimmed", typeof(bool), typeof(DataPoint), new PropertyMetadata(false, new PropertyChangedCallback(DataPoint.OnIsDimmedPropertyChanged)));
        public static readonly DependencyProperty ZIndexProperty = DependencyProperty.Register("ZIndex", typeof(int), typeof(DataPoint), new PropertyMetadata(0, new PropertyChangedCallback(DataPoint.OnZIndexPropertyChanged)));
        public static readonly DependencyProperty UnselectedOpacityProperty = DependencyProperty.Register("UnselectedOpacity", typeof(double), typeof(DataPoint), new PropertyMetadata(1.0, new PropertyChangedCallback(DataPoint.OnUnselectedOpacityPropertyChanged)));
        public static readonly DependencyProperty UnselectedEffectProperty = DependencyProperty.Register("UnselectedEffect", typeof(Effect), typeof(DataPoint), new PropertyMetadata(null, new PropertyChangedCallback(DataPoint.OnUnselectedEffectPropertyChanged)));
        public static readonly DependencyProperty DataPointProperty = DependencyProperty.RegisterAttached("DataPoint", typeof(DataPoint), typeof(DataPoint), new PropertyMetadata(null, new PropertyChangedCallback(DataPoint.OnDataPointPropertyChanged)));
        public static readonly DependencyProperty LabelStyleProperty = DependencyProperty.Register("LabelStyle", typeof(Style), typeof(DataPoint), new PropertyMetadata(null, new PropertyChangedCallback(DataPoint.OnLabelStyleChanged)));
        public static readonly DependencyProperty LabelContentProperty = DependencyProperty.Register("LabelContent", typeof(object), typeof(DataPoint), new PropertyMetadata(null, new PropertyChangedCallback(DataPoint.OnLabelContentChanged)));
        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register("StringFormat", typeof(string), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnStringFormatChanged)));
        public static readonly DependencyProperty ShowValueAsLabelProperty = DependencyProperty.Register("ShowValueAsLabel", typeof(bool), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnShowValueAsLabelChanged)));
        public static readonly DependencyProperty LabelMarginProperty = DependencyProperty.Register("LabelMargin", typeof(double), typeof(DataPoint), new PropertyMetadata(5.0, new PropertyChangedCallback(DataPoint.OnLabelMarginChanged)));
        public static readonly DependencyProperty LabelRotationProperty = DependencyProperty.Register("LabelRotation", typeof(double), typeof(DataPoint), new PropertyMetadata(0.0, new PropertyChangedCallback(DataPoint.OnLabelRotationChanged)));
        public static readonly DependencyProperty MarkerStyleProperty = DependencyProperty.Register("MarkerStyle", typeof(Style), typeof(DataPoint), new PropertyMetadata(null, new PropertyChangedCallback(DataPoint.OnMarkerStyleChanged)));
        public static readonly DependencyProperty MarkerTypeProperty = DependencyProperty.Register("MarkerType", typeof(MarkerType), typeof(DataPoint), new PropertyMetadata(MarkerType.None, new PropertyChangedCallback(DataPoint.OnMarkerTypeChanged)));
        public static readonly DependencyProperty MarkerSizeProperty = DependencyProperty.Register("MarkerSize", typeof(double), typeof(DataPoint), new PropertyMetadata(10.0, new PropertyChangedCallback(DataPoint.OnMarkerSizeChanged)));
        public static readonly DependencyProperty ToolTipContentProperty = DependencyProperty.Register("ToolTipContent", typeof(IAutomationNameProvider), typeof(DataPoint), new PropertyMetadata(null, new PropertyChangedCallback(DataPoint.OnToolTipContentPropertyChanged)));
        public static readonly DependencyProperty ToolTipStyleProperty = DependencyProperty.Register("ToolTipStyle", typeof(Style), typeof(DataPoint), new PropertyMetadata(null, new PropertyChangedCallback(DataPoint.OnToolTipStylePropertyChanged)));
        private double _actualOpacity = 1.0;
        private Visibility _actualLabelVisibility = Visibility.Collapsed;
        private double _actualLabelMargin = 5.0;
        internal const string OpacityPropertyName = "Opacity";
        internal const string EffectPropertyName = "Effect";
        internal const string DataContextPropertyName = "DataContext";
        internal const string FillPropertyName = "Fill";
        internal const string StrokePropertyName = "Stroke";
        internal const string StrokeThicknessPropertyName = "StrokeThickness";
        internal const string StrokeDashTypePropertyName = "StrokeDashType";
        internal const string IsEmptyPropertyName = "IsEmpty";
        internal const string IsSelectedPropertyName = "IsSelected";
        internal const string IsDimmedPropertyName = "IsDimmed";
        internal const string ZIndexPropertyName = "ZIndex";
        internal const string UnselectedOpacityPropertyName = "UnselectedOpacity";
        internal const string UnselectedEffectPropertyName = "UnselectedEffect";
        internal const string DataPointPropertyName = "DataPoint";
        internal const string ViewPropertyName = "View";
        internal const string ViewStatePropertyName = "ViewState";
        internal const string ActualStylePropertyName = "ActualStyle";
        internal const string ActualEffectPropertyName = "ActualEffect";
        internal const string ActualOpacityPropertyName = "ActualOpacity";
        internal const double DefaultLabelMargin = 5.0;
        internal const string LabelStylePropertyName = "LabelStyle";
        internal const string LabelContentPropertyName = "LabelContent";
        internal const string ActualLabelContentPropertyName = "ActualLabelContent";
        internal const string ActualLabelVisibilityPropertyName = "ActualLabelVisibility";
        internal const string StringFormatPropertyName = "StringFormat";
        internal const string ShowValueAsLabelPropertyName = "ShowValueAsLabel";
        internal const string LabelMarginPropertyName = "LabelMargin";
        internal const string ActualLabelMarginPropertyName = "ActualLabelMargin";
        internal const string LabelRotationPropertyName = "LabelRotation";
        internal const double DefaultMarkerSize = 10.0;
        internal const string MarkerStylePropertyName = "MarkerStyle";
        internal const string MarkerTypePropertyName = "MarkerType";
        internal const string MarkerSizePropertyName = "MarkerSize";
        internal const string ToolTipContentPropertyName = "ToolTipContent";
        internal const string ToolTipStylePropertyName = "ToolTipStyle";
        private bool _isNotificationBindingSet;
        private Series _series;
        private DataPointView _view;
        private Dictionary<string, StoryboardInfo> _storyboards;
        private DataPointViewState _viewState;
        private Style _actualDataPointStyle;
        private Effect _actualEffect;
        private object _actualLabelContent;

        internal bool IsNewlyAdded { get; set; }

        internal new bool IsVisible { get; set; }

        public Series Series
        {
            get
            {
                return this._series;
            }
            internal set
            {
                if (this._series == value)
                    return;
                if (this._series != null && value != null)
                    throw new InvalidOperationException(Properties.Resources.DataPointSeriesCannotBeChanged);
                this._series = value;
                if (this._series == null)
                    return;
                this.UpdateBinding();
                this.UpdateActualPropertiesFromSeries(null);
            }
        }

        protected int CallNestLevelFromRoot { get; set; }

        public Brush Fill
        {
            get
            {
                return this.GetValue(DataPoint.FillProperty) as Brush;
            }
            set
            {
                this.SetValue(DataPoint.FillProperty, value);
            }
        }

        public Brush Stroke
        {
            get
            {
                return this.GetValue(DataPoint.StrokeProperty) as Brush;
            }
            set
            {
                this.SetValue(DataPoint.StrokeProperty, value);
            }
        }

        public double StrokeThickness
        {
            get
            {
                return (double)this.GetValue(DataPoint.StrokeThicknessProperty);
            }
            set
            {
                this.SetValue(DataPoint.StrokeThicknessProperty, value);
            }
        }

        public StrokeDashType StrokeDashType
        {
            get
            {
                return (StrokeDashType)this.GetValue(DataPoint.StrokeDashTypeProperty);
            }
            set
            {
                this.SetValue(DataPoint.StrokeDashTypeProperty, value);
            }
        }

        protected abstract object PrimaryValue { get; }

        public bool IsEmpty
        {
            get
            {
                return (bool)this.GetValue(DataPoint.IsEmptyProperty);
            }
            set
            {
                this.SetValue(DataPoint.IsEmptyProperty, value);
            }
        }

        public virtual bool ActualIsEmpty
        {
            get
            {
                return this.PrimaryValue == null;
            }
        }

        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(DataPoint.IsSelectedProperty);
            }
            set
            {
                this.SetValue(DataPoint.IsSelectedProperty, value);
            }
        }

        public bool IsDimmed
        {
            get
            {
                return (bool)this.GetValue(DataPoint.IsDimmedProperty);
            }
            set
            {
                this.SetValue(DataPoint.IsDimmedProperty, value);
            }
        }

        public int ZIndex
        {
            get
            {
                return (int)this.GetValue(DataPoint.ZIndexProperty);
            }
            set
            {
                this.SetValue(DataPoint.ZIndexProperty, value);
            }
        }

        public double UnselectedOpacity
        {
            get
            {
                return (double)this.GetValue(DataPoint.UnselectedOpacityProperty);
            }
            set
            {
                this.SetValue(DataPoint.UnselectedOpacityProperty, value);
            }
        }

        public Effect UnselectedEffect
        {
            get
            {
                return (Effect)this.GetValue(DataPoint.UnselectedEffectProperty);
            }
            set
            {
                this.SetValue(DataPoint.UnselectedEffectProperty, value);
            }
        }

        internal DataPointView View
        {
            get
            {
                return this._view;
            }
            set
            {
                if (this._view == value)
                    return;
                DataPointView dataPointView = this._view;
                this._view = value;
                if (this._view != null)
                    this._view.DataPoint = this;
                this.OnValueChanged("View", dataPointView, this._view);
            }
        }

        internal Dictionary<string, StoryboardInfo> Storyboards
        {
            get
            {
                if (this._storyboards == null)
                    this._storyboards = new Dictionary<string, StoryboardInfo>();
                return this._storyboards;
            }
        }

        internal DataPointViewState ViewState
        {
            get
            {
                return this._viewState;
            }
            set
            {
                if (this._viewState == value)
                    return;
                DataPointViewState dataPointViewState = this._viewState;
                this._viewState = value;
                this.OnValueChanged("ViewState", dataPointViewState, this._viewState);
            }
        }

        public Style ActualDataPointStyle
        {
            get
            {
                return this._actualDataPointStyle;
            }
            private set
            {
                if (this._actualDataPointStyle == value)
                    return;
                Style style = this._actualDataPointStyle;
                this._actualDataPointStyle = value;
                this.Style = this._actualDataPointStyle;
                this.OnValueChanged("ActualStyle", style, this._actualDataPointStyle);
            }
        }

        double IAppearanceProvider.Opacity
        {
            get
            {
                return this.ActualOpacity;
            }
        }

        Effect IAppearanceProvider.Effect
        {
            get
            {
                return this.ActualEffect;
            }
        }

        public Effect ActualEffect
        {
            get
            {
                return this._actualEffect;
            }
            internal set
            {
                if (this._actualEffect == value)
                    return;
                Effect effect = this._actualEffect;
                this._actualEffect = value;
                this.OnValueChanged("ActualEffect", effect, this._actualEffect);
            }
        }

        public double ActualOpacity
        {
            get
            {
                return this._actualOpacity;
            }
            internal set
            {
                if (this._actualOpacity == value)
                    return;
                double num = this._actualOpacity;
                this._actualOpacity = value;
                this.OnValueChanged("ActualOpacity", num, this._actualOpacity);
            }
        }

        IUpdatable IUpdatable.Parent
        {
            get
            {
                return this.Series;
            }
        }

        public UpdateSession UpdateSession
        {
            get
            {
                if (this.Series != null && this.Series.ChartArea != null)
                    return this.Series.ChartArea.UpdateSession;
                return null;
            }
        }

        public Style LabelStyle
        {
            get
            {
                return this.GetValue(DataPoint.LabelStyleProperty) as Style;
            }
            set
            {
                this.SetValue(DataPoint.LabelStyleProperty, value);
            }
        }

        public object LabelContent
        {
            get
            {
                return this.GetValue(DataPoint.LabelContentProperty);
            }
            set
            {
                this.SetValue(DataPoint.LabelContentProperty, value);
            }
        }

        public object ActualLabelContent
        {
            get
            {
                return this._actualLabelContent;
            }
            private set
            {
                if (this._actualLabelContent == value)
                    return;
                object oldValue = this._actualLabelContent;
                this._actualLabelContent = value;
                if ((oldValue == null && this._actualLabelContent != null || oldValue != null && this._actualLabelContent == null) && (this.Series != null && this.Series.ChartArea != null))
                    this.Series.SeriesPresenter.InvalidateDataPointLabel(this);
                this.OnValueChanged("ActualLabelContent", oldValue, this._actualLabelContent);
            }
        }

        internal Visibility ActualLabelVisibility
        {
            get
            {
                return this._actualLabelVisibility;
            }
            set
            {
                if (this._actualLabelVisibility == value)
                    return;
                this._actualLabelVisibility = value;
                if (this.Series != null && this.Series.ChartArea != null)
                    this.Series.SeriesPresenter.InvalidateDataPointLabel(this);
                this.OnPropertyChanged("ActualLabelVisibility");
            }
        }

        public string StringFormat
        {
            get
            {
                return this.GetValue(DataPoint.StringFormatProperty) as string;
            }
            set
            {
                this.SetValue(DataPoint.StringFormatProperty, value);
            }
        }

        public bool ShowValueAsLabel
        {
            get
            {
                return (bool)this.GetValue(DataPoint.ShowValueAsLabelProperty);
            }
            set
            {
                this.SetValue(DataPoint.ShowValueAsLabelProperty, value);
            }
        }

        public double LabelMargin
        {
            get
            {
                return (double)this.GetValue(DataPoint.LabelMarginProperty);
            }
            set
            {
                this.SetValue(DataPoint.LabelMarginProperty, value);
            }
        }

        public double ActualLabelMargin
        {
            get
            {
                return this._actualLabelMargin;
            }
            private set
            {
                if (this._actualLabelMargin == value)
                    return;
                double num = this._actualLabelMargin;
                this._actualLabelMargin = value;
                this.OnValueChanged("ActualLabelMargin", num, this._actualLabelMargin);
            }
        }

        public double LabelRotation
        {
            get
            {
                return (double)this.GetValue(DataPoint.LabelRotationProperty);
            }
            set
            {
                this.SetValue(DataPoint.LabelRotationProperty, value);
            }
        }

        public Style MarkerStyle
        {
            get
            {
                return this.GetValue(DataPoint.MarkerStyleProperty) as Style;
            }
            set
            {
                this.SetValue(DataPoint.MarkerStyleProperty, value);
            }
        }

        public MarkerType MarkerType
        {
            get
            {
                return (MarkerType)this.GetValue(DataPoint.MarkerTypeProperty);
            }
            set
            {
                this.SetValue(DataPoint.MarkerTypeProperty, value);
            }
        }

        public double MarkerSize
        {
            get
            {
                return (double)this.GetValue(DataPoint.MarkerSizeProperty);
            }
            set
            {
                this.SetValue(DataPoint.MarkerSizeProperty, value);
            }
        }

        public IAutomationNameProvider ToolTipContent
        {
            get
            {
                return (IAutomationNameProvider)this.GetValue(DataPoint.ToolTipContentProperty);
            }
            set
            {
                this.SetValue(DataPoint.ToolTipContentProperty, value);
            }
        }

        public Style ToolTipStyle
        {
            get
            {
                return (Style)this.GetValue(DataPoint.ToolTipStyleProperty);
            }
            set
            {
                this.SetValue(DataPoint.ToolTipStyleProperty, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event ValueChangedEventHandler ValueChanged;

        private static void OnFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            Brush oldValue = (Brush)e.OldValue;
            ((DataPoint)o).OnFillChanged(oldValue, newValue);
        }

        protected virtual void OnFillChanged(Brush oldValue, Brush newValue)
        {
            this.OnValueChanged("Fill", oldValue, newValue);
        }

        private static void OnStrokeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            Brush oldValue = (Brush)e.OldValue;
            ((DataPoint)o).OnStrokeChanged(oldValue, newValue);
        }

        protected virtual void OnStrokeChanged(Brush oldValue, Brush newValue)
        {
            this.OnValueChanged("Stroke", oldValue, newValue);
        }

        private static void OnStrokeThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            double newValue = (double)e.NewValue;
            double oldValue = (double)e.OldValue;
            ((DataPoint)o).OnStrokeThicknessChanged(oldValue, newValue);
        }

        protected virtual void OnStrokeThicknessChanged(double oldValue, double newValue)
        {
            if (this.CallNestLevelFromRoot != 0)
                return;
            if (newValue < 0.0)
            {
                ++this.CallNestLevelFromRoot;
                this.StrokeThickness = oldValue;
                --this.CallNestLevelFromRoot;
                throw new ArgumentOutOfRangeException(Properties.Resources.TheStrokeThicknessPropertyValueCannotBeANegativeNumber);
            }
            this.OnValueChanged("StrokeThickness", oldValue, newValue);
        }

        private static void OnStrokeDashTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StrokeDashType newValue = (StrokeDashType)e.NewValue;
            StrokeDashType oldValue = (StrokeDashType)e.OldValue;
            ((DataPoint)o).OnStrokeDashTypeChanged(oldValue, newValue);
        }

        protected virtual void OnStrokeDashTypeChanged(StrokeDashType oldValue, StrokeDashType newValue)
        {
            this.OnValueChanged("StrokeDashType", oldValue, newValue);
        }

        private static void OnIsEmptyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            bool oldValue = (bool)e.OldValue;
            ((DataPoint)o).OnIsEmptyChanged(oldValue, newValue);
        }

        protected virtual void OnIsEmptyChanged(bool oldValue, bool newValue)
        {
            this.OnValueChanged("IsEmpty", oldValue, newValue);
        }

        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.OldValue == (bool)e.NewValue)
                return;
            ((DataPoint)d).OnIsSelectedPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsSelectedPropertyChanged(object oldValue, object newValue)
        {
            this.OnValueChanged("IsSelected", oldValue, newValue);
        }

        private static void OnIsDimmedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPoint)d).OnIsDimmedPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsDimmedPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("IsDimmed");
        }

        private static void OnZIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPoint)d).OnZIndexPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnZIndexPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ZIndex");
        }

        private static void OnUnselectedOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPoint)d).OnUnselectedOpacityPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnUnselectedOpacityPropertyChanged(object oldValue, object newValue)
        {
            this.OnValueChanged("UnselectedOpacity", oldValue, newValue);
        }

        private static void OnUnselectedEffectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPoint)d).OnUnselectedEffectPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnUnselectedEffectPropertyChanged(object oldValue, object newValue)
        {
            this.OnValueChanged("UnselectedEffect", oldValue, newValue);
        }

        public static DataPoint GetDataPoint(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (DataPoint)element.GetValue(DataPoint.DataPointProperty);
        }

        public static void SetDataPoint(DependencyObject element, DataPoint value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(DataPoint.DataPointProperty, value);
        }

        private static void OnDataPointPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal void Show()
        {
            switch (this.ViewState)
            {
                case DataPointViewState.Hidden:
                case DataPointViewState.Hiding:
                    this.ViewState = DataPointViewState.Showing;
                    break;
            }
        }

        internal void Hide()
        {
            switch (this.ViewState)
            {
                case DataPointViewState.Showing:
                case DataPointViewState.Normal:
                    this.ViewState = DataPointViewState.Hiding;
                    break;
            }
        }

        public void Update()
        {
            if (this.Series == null || this.Series.ChartArea == null)
                return;
            this.Series.SeriesPresenter.UpdateDataPoint(this);
        }

        internal virtual void UpdateActualPropertiesFromSeries(string name)
        {
            if (this.Series == null)
                return;
            switch (name)
            {
                case "DataPointStyle":
                case "EmptyDataPointStyle":
                    this.UpdateActualDataPointStyle();
                    break;
                case "Effect":
                    this.UpdateActualDataPointEffect();
                    break;
                case "Opacity":
                    this.UpdateActualDataPointOpacity();
                    break;
                case "UnselectedDataPointEffect":
                case "HaveSelectedDataPoints":
                case "IsDimmed":
                    this.UpdateActualDataPointEffect();
                    this.UpdateActualDataPointOpacity();
                    break;
                case "LabelDisplayUnitSystem":
                    this.UpdateActualLabelContent();
                    break;
                case null:
                    this.UpdateActualDataPointStyle();
                    this.UpdateActualDataPointEffect();
                    this.UpdateActualDataPointOpacity();
                    this.UpdateActualLabelContent();
                    break;
            }
        }

        protected virtual void UpdateActualPropertiesFromDataPoint(string propertyName)
        {
            if (propertyName == null || propertyName == "MarkerType" || (propertyName == "MarkerSize" || propertyName == "LabelMargin"))
                this.UpdateActualLabelMargin();
            if (propertyName == null || propertyName == "LabelContent" || (propertyName == "ShowValueAsLabel" || propertyName == "StringFormat"))
                this.UpdateActualLabelContent();
            if (propertyName == null || propertyName == "IsEmpty")
                this.UpdateActualDataPointStyle();
            if (propertyName == null || propertyName == "Effect" || (propertyName == "UnselectedEffect" || propertyName == "IsDimmed"))
                this.UpdateActualDataPointEffect();
            if (propertyName == null || propertyName == "Opacity" || (propertyName == "UnselectedOpacity" || propertyName == "IsDimmed"))
                this.UpdateActualDataPointOpacity();
            if (propertyName == null || propertyName == "ZIndex")
            {
                FrameworkElement dataPointView = SeriesVisualStatePresenter.GetDataPointView(this);
                if (dataPointView != null)
                    Panel.SetZIndex(dataPointView, this.ZIndex);
            }
            if (propertyName == null || propertyName == "IsSelected")
            {
                if (this.Series != null && this.Series.SeriesPresenter != null)
                    this.Series.SeriesPresenter.UpdateDataPointZIndex(this);
                if (this.Series != null)
                {
                    this.Series.UpdateSelectedDataPointFlag(this);
                    this.UpdateActualDataPointOpacity();
                    this.UpdateActualDataPointEffect();
                }
            }
            if (propertyName != null && !(propertyName == "ViewState"))
                return;
            if (this.ViewState == DataPointViewState.Hidden)
            {
                if (this.Series == null || this.Series.DataPoints.Contains(this))
                    return;
                this.IsSelected = false;
            }
            else
            {
                if (this.ViewState != DataPointViewState.Showing || !this.IsNewlyAdded || (!this.IsSelected || this.Series == null))
                    return;
                this.Series.UpdateSelectedDataPointFlag(this);
            }
        }

        private void UpdateActualDataPointStyle()
        {
            if (this.Series == null)
                return;
            if (this.IsEmpty && this.Series.EmptyDataPointStyle != null)
                this.ActualDataPointStyle = this.Series.EmptyDataPointStyle;
            else
                this.ActualDataPointStyle = this.Series.DataPointStyle;
        }

        private void UpdateActualDataPointEffect()
        {
            Effect effect = null;
            if (this.Series != null && this.Series.IsDataPointAppearsUnselected(this))
            {
                if (this.UnselectedEffect != null)
                    effect = this.UnselectedEffect;
                else if (this.Series != null)
                    effect = this.Series.UnselectedDataPointEffect;
            }
            if (effect == null)
            {
                if (this.Effect != null)
                    effect = this.Effect;
                else if (this.Series != null)
                    effect = this.Series.Effect;
            }
            this.ActualEffect = effect;
        }

        private void UpdateActualDataPointOpacity()
        {
            double opacity = this.Opacity;
            double num = this.Series == null || !this.Series.IsDataPointAppearsUnselected(this) ? (this.Series == null ? this.Opacity : Math.Min(this.Series.Opacity, this.Opacity)) : Math.Min(this.Series.UnselectedDataPointOpacity, this.UnselectedOpacity);
            string storyboardKey = DependencyPropertyAnimationHelper.GetStoryboardKey("ActualOpacity");
            StoryboardInfo storyboardInfo = null;
            if (this.Storyboards.TryGetValue(storyboardKey, out storyboardInfo) && storyboardInfo.Storyboard.Children.Count > 0)
            {
                DoubleAnimation doubleAnimation = storyboardInfo.Storyboard.Children[0] as DoubleAnimation;
                if (doubleAnimation != null)
                {
                    doubleAnimation.To = new double?(num);
                    return;
                }
            }
            this.ActualOpacity = num;
        }

        private void UpdateActualLabelMargin()
        {
            this.UpdateActualLabelMargin(this.MarkerSize);
        }

        internal virtual void UpdateActualLabelMargin(double markerSize)
        {
            if (ValueHelper.CanGraph(markerSize))
            {
                double labelMargin = this.LabelMargin;
                if (this.MarkerType != MarkerType.None)
                    labelMargin += markerSize / 2.0;
                this.ActualLabelMargin = labelMargin;
            }
            else
                this.ActualLabelMargin = 0.0;
        }

        internal void UpdateActualLabelContent()
        {
            if (this.LabelContent != null)
                this.ActualLabelContent = this.LabelContent;
            else if (this.ShowValueAsLabel)
            {
                StringFormatConverter stringFormatConverter = new StringFormatConverter();
                string str = ValueHelper.PrepareFormatString(this.StringFormat);
                bool flag = false;
                double doubleValue;
                if (this.Series != null && this.Series.LabelDisplayUnitSystem != null && ValueHelper.TryConvert(this.PrimaryValue, false, out doubleValue))
                {
                    this.Series.LabelDisplayUnitSystem.CalculateActualDisplayUnit(doubleValue, str);
                    if (this.Series.LabelDisplayUnitSystem.ActualDisplayUnit != null)
                    {
                        flag = true;
                        this.ActualLabelContent = this.Series.LabelDisplayUnitSystem.FormatLabel(str, doubleValue, new int?(2));
                    }
                }
                if (flag)
                    return;
                this.ActualLabelContent = stringFormatConverter.Convert(this.PrimaryValue, null, str, null);
            }
            else
                this.ActualLabelContent = null;
        }

        internal void OnPropertyChanged(string propertyName)
        {
            this.UpdateActualPropertiesFromDataPoint(propertyName);
            if (this.PropertyChanged == null)
                return;
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void OnValueChanged(string propertyName, object oldValue, object newValue)
        {
            this.OnPropertyChanged(propertyName);
            if (this.ValueChanged == null)
                return;
            this.ValueChanged(this, new ValueChangedEventArgs(propertyName, oldValue, newValue));
        }

        internal virtual void CreateNotificationBindings()
        {
            if (this._isNotificationBindingSet)
                return;
            Binding binding1 = new Binding("Opacity") { Source = (this) };
            this.SetBinding(Series.OpacityListenerProperty, binding1);
            Binding binding2 = new Binding("Effect") { Source = (this) };
            this.SetBinding(Series.EffectListenerProperty, binding2);
            this._isNotificationBindingSet = true;
        }

        internal virtual void UpdateBinding()
        {
            if (this.Series == null || this.Series.ItemsBinder != null)
                return;
            this.SetDataPointBinding(DataPoint.LabelContentProperty, this.Series.LabelContentBinding);
            this.SetDataPointBinding(DataPoint.ToolTipContentProperty, this.Series.ToolTipContentBinding);
        }

        internal void SetDataPointBinding(DependencyProperty dp, Binding binding)
        {
            if (this.DataContext == null)
                return;
            if (binding != null)
                this.SetBinding(dp, binding);
            else
                this.ClearBinding(dp);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataPointAutomationPeer(this);
        }

        private static void OnLabelStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Style newValue = (Style)e.NewValue;
            Style oldValue = (Style)e.OldValue;
            ((DataPoint)o).OnLabelStyleChanged(oldValue, newValue);
        }

        protected virtual void OnLabelStyleChanged(Style oldValue, Style newValue)
        {
            this.OnValueChanged("LabelStyle", oldValue, newValue);
        }

        private static void OnLabelContentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((DataPoint)o).OnLabelContentChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnLabelContentChanged(object oldValue, object newValue)
        {
            this.OnValueChanged("LabelContent", oldValue, newValue);
        }

        private static void OnStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            string newValue = (string)e.NewValue;
            string oldValue = (string)e.OldValue;
            ((DataPoint)o).OnStringFormatChanged(oldValue, newValue);
        }

        protected virtual void OnStringFormatChanged(string oldValue, string newValue)
        {
            this.OnValueChanged("StringFormat", oldValue, newValue);
        }

        private static void OnShowValueAsLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            bool oldValue = (bool)e.OldValue;
            ((DataPoint)o).OnShowValueAsLabelChanged(oldValue, newValue);
        }

        protected virtual void OnShowValueAsLabelChanged(bool oldValue, bool newValue)
        {
            this.OnValueChanged("ShowValueAsLabel", oldValue, newValue);
        }

        private static void OnLabelMarginChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            double newValue = (double)e.NewValue;
            double oldValue = (double)e.OldValue;
            ((DataPoint)o).OnLabelMarginChanged(oldValue, newValue);
        }

        protected virtual void OnLabelMarginChanged(double oldValue, double newValue)
        {
            this.OnValueChanged("LabelMargin", oldValue, newValue);
        }

        private static void OnLabelRotationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            double newValue = (double)e.NewValue;
            double oldValue = (double)e.OldValue;
            ((DataPoint)o).OnLabelRotationChanged(oldValue, newValue);
        }

        protected virtual void OnLabelRotationChanged(double oldValue, double newValue)
        {
            this.OnValueChanged("LabelRotation", oldValue, newValue);
        }

        private static void OnMarkerStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Style newValue = (Style)e.NewValue;
            Style oldValue = (Style)e.OldValue;
            ((DataPoint)o).OnMarkerStyleChanged(oldValue, newValue);
        }

        protected virtual void OnMarkerStyleChanged(Style oldValue, Style newValue)
        {
            this.OnValueChanged("MarkerStyle", oldValue, newValue);
        }

        private static void OnMarkerTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MarkerType newValue = (MarkerType)e.NewValue;
            MarkerType oldValue = (MarkerType)e.OldValue;
            ((DataPoint)o).OnMarkerTypeChanged(oldValue, newValue);
        }

        private void OnMarkerTypeChanged(MarkerType oldValue, MarkerType newValue)
        {
            this.OnValueChanged("MarkerType", oldValue, newValue);
        }

        private static void OnMarkerSizeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            double newValue = (double)e.NewValue;
            double oldValue = (double)e.OldValue;
            ((DataPoint)o).OnMarkerSizeChanged(oldValue, newValue);
        }

        protected virtual void OnMarkerSizeChanged(double oldValue, double newValue)
        {
            if (this.CallNestLevelFromRoot != 0)
                return;
            if (newValue < 0.0)
            {
                ++this.CallNestLevelFromRoot;
                this.MarkerSize = oldValue;
                --this.CallNestLevelFromRoot;
                throw new ArgumentOutOfRangeException(Properties.Resources.Marker_size_must_be_a_positive_number);
            }
            this.OnValueChanged("MarkerSize", oldValue, newValue);
        }

        private static void OnToolTipContentPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((DataPoint)o).OnToolTipContentChanged((IAutomationNameProvider)e.OldValue, (IAutomationNameProvider)e.NewValue);
        }

        protected virtual void OnToolTipContentChanged(IAutomationNameProvider oldValue, IAutomationNameProvider newValue)
        {
            this.OnValueChanged("ToolTipContent", oldValue, newValue);
        }

        private static void OnToolTipStylePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Style newValue = (Style)e.NewValue;
            Style oldValue = (Style)e.OldValue;
            ((DataPoint)o).OnToolTipStyleChanged(oldValue, newValue);
        }

        protected virtual void OnToolTipStyleChanged(Style oldValue, Style newValue)
        {
            this.OnValueChanged("ToolTipStyle", oldValue, newValue);
        }
    }
}
