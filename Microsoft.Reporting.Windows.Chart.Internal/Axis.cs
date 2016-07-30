using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    [StyleTypedProperty(Property = "DisplayUnitTitleStyle", StyleTargetType = typeof(Microsoft.Reporting.Windows.Common.Internal.Title))]
    [StyleTypedProperty(Property = "ScrollZoomBarStyle", StyleTargetType = typeof(ScrollZoomBar))]
    [StyleTypedProperty(Property = "MajorGridlineStyle", StyleTargetType = typeof(Shape))]
    [StyleTypedProperty(Property = "LineStyle", StyleTargetType = typeof(Shape))]
    [StyleTypedProperty(Property = "MajorTickMarkStyle", StyleTargetType = typeof(TickMark))]
    [StyleTypedProperty(Property = "TitleStyle", StyleTargetType = typeof(Microsoft.Reporting.Windows.Common.Internal.Title))]
    [StyleTypedProperty(Property = "LabelStyle", StyleTargetType = typeof(AxisLabelControl))]
    [StyleTypedProperty(Property = "MinorTickMarkStyle", StyleTargetType = typeof(TickMark))]
    [StyleTypedProperty(Property = "MinorGridlineStyle", StyleTargetType = typeof(Shape))]
    public class Axis : Control, IAxisPresenterProvider, INotifyPropertyChanged, IUpdatable
    {
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(Scale), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnScalePropertyChanged)));
        public static readonly DependencyProperty LineStyleProperty = DependencyProperty.Register("LineStyle", typeof(Style), typeof(Axis), null);
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnTitlePropertyChanged)));
        public static readonly DependencyProperty ActualDisplayUnitTitleProperty = DependencyProperty.Register("ActualDisplayUnitTitle", typeof(object), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnActualDisplayUnitTitlePropertyChanged)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(AxisOrientation), typeof(Axis), new PropertyMetadata(AxisOrientation.None, new PropertyChangedCallback(Axis.OnOrientationPropertyChanged)));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(AxisPosition), typeof(Axis), new PropertyMetadata(AxisPosition.Auto, new PropertyChangedCallback(Axis.OnPositionPropertyChanged)));
        public static readonly DependencyProperty IsReversedProperty = DependencyProperty.Register("IsReversed", typeof(bool), typeof(Axis), new PropertyMetadata(false, new PropertyChangedCallback(Axis.OnIsReversedPropertyChanged)));
        public static readonly DependencyProperty IsScrollZoomBarAllowsZoomingProperty = DependencyProperty.Register("IsScrollZoomBarAllowsZooming", typeof(bool), typeof(Axis), new PropertyMetadata(true, new PropertyChangedCallback(Axis.OnIsScrollZoomBarAllowsZoomingPropertyChanged)));
        public static readonly DependencyProperty ActualIsZoomEnabledProperty = DependencyProperty.Register("ActualIsZoomEnabled", typeof(bool), typeof(Axis), new PropertyMetadata(true, new PropertyChangedCallback(Axis.OnActualIsZoomEnabledPropertyChanged)));
        public static readonly DependencyProperty IsScrollZoomBarAllwaysMaximizedProperty = DependencyProperty.Register("IsScrollZoomBarAllwaysMaximized", typeof(bool), typeof(Axis), new PropertyMetadata(false, new PropertyChangedCallback(Axis.OnIsScrollZoomBarAllwaysMaximizedPropertyChanged)));
        public static readonly DependencyProperty IsAllowsAutoZoomProperty = DependencyProperty.Register("IsAllowsAutoZoom", typeof(bool), typeof(Axis), new PropertyMetadata(false, new PropertyChangedCallback(Axis.OnIsAllowsAutoZoomPropertyChanged)));
        public static readonly DependencyProperty ViewSizeInPercentProperty = DependencyProperty.Register("ViewSizeInPercent", typeof(double), typeof(Axis), new PropertyMetadata(1.0, new PropertyChangedCallback(Axis.OnViewSizeInPercentPropertyChanged)));
        public static readonly DependencyProperty ViewPositionInPercentProperty = DependencyProperty.Register("ViewPositionInPercent", typeof(double), typeof(Axis), new PropertyMetadata(0.0, new PropertyChangedCallback(Axis.OnViewPositionInPercentPropertyChanged)));
        public static readonly DependencyProperty IsMarginVisibleProperty = DependencyProperty.Register("IsMarginVisible", typeof(AutoBool), typeof(Axis), new PropertyMetadata(AutoBool.Auto, new PropertyChangedCallback(Axis.OnIsMarginVisiblePropertyChanged)));
        public static readonly DependencyProperty TitleStyleProperty = DependencyProperty.Register("TitleStyle", typeof(Style), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnTitleStyleChanged)));
        public static readonly DependencyProperty DisplayUnitTitleStyleProperty = DependencyProperty.Register("DisplayUnitTitleStyle", typeof(Style), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnDisplayUnitTitleStyleChanged)));
        public static readonly DependencyProperty StrokeDashTypeProperty = DependencyProperty.Register("StrokeDashType", typeof(StrokeDashType), typeof(Axis), new PropertyMetadata(StrokeDashType.Solid, new PropertyChangedCallback(Axis.OnStrokeDashTypePropertyChanged)));
        public static readonly DependencyProperty MajorGridlineStyleProperty = DependencyProperty.Register("MajorGridlineStyle", typeof(Style), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnMajorGridlineStylePropertyChanged)));
        public static readonly DependencyProperty MajorTickMarkStyleProperty = DependencyProperty.Register("MajorTickMarkStyle", typeof(Style), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnMajorTickMarkStylePropertyChanged)));
        public static readonly DependencyProperty MinorGridlineStyleProperty = DependencyProperty.Register("MinorGridlineStyle", typeof(Style), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnMinorGridlineStylePropertyChanged)));
        public static readonly DependencyProperty MinorTickMarkStyleProperty = DependencyProperty.Register("MinorTickMarkStyle", typeof(Style), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnMinorTickMarkStylePropertyChanged)));
        public static readonly DependencyProperty LabelStyleProperty = DependencyProperty.Register("LabelStyle", typeof(Style), typeof(Axis), new PropertyMetadata(new PropertyChangedCallback(Axis.OnLabelStylePropertyChanged)));
        public static readonly DependencyProperty ScrollZoomBarStyleProperty = DependencyProperty.Register("ScrollZoomBarStyle", typeof(Style), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnScrollZoomBarStylePropertyChanged)));
        public static readonly DependencyProperty ShowMajorGridlinesProperty = DependencyProperty.Register("ShowMajorGridlines", typeof(bool), typeof(Axis), new PropertyMetadata(true, new PropertyChangedCallback(Axis.OnShowMajorGridlinesPropertyChanged)));
        public static readonly DependencyProperty ShowMajorTickMarksProperty = DependencyProperty.Register("ShowMajorTickMarks", typeof(bool), typeof(Axis), new PropertyMetadata(true, new PropertyChangedCallback(Axis.OnShowMajorTickMarksPropertyChanged)));
        public static readonly DependencyProperty ShowLabelsProperty = DependencyProperty.Register("ShowLabels", typeof(bool), typeof(Axis), new PropertyMetadata(true, new PropertyChangedCallback(Axis.OnShowLabelsPropertyChanged)));
        public static readonly DependencyProperty ShowTitlesProperty = DependencyProperty.Register("ShowTitles", typeof(bool), typeof(Axis), new PropertyMetadata(true, new PropertyChangedCallback(Axis.OnShowTitlesPropertyChanged)));
        public static readonly DependencyProperty ShowAxisLineProperty = DependencyProperty.Register("ShowAxisLine", typeof(bool), typeof(Axis), new PropertyMetadata(true, new PropertyChangedCallback(Axis.OnShowAxisLinePropertyChanged)));
        public static readonly DependencyProperty ShowMinorGridlinesProperty = DependencyProperty.Register("ShowMinorGridlines", typeof(AutoBool), typeof(Axis), new PropertyMetadata(AutoBool.Auto, new PropertyChangedCallback(Axis.OnShowMinorGridlinesPropertyChanged)));
        public static readonly DependencyProperty ShowMinorTickMarksProperty = DependencyProperty.Register("ShowMinorTickMarks", typeof(AutoBool), typeof(Axis), new PropertyMetadata(AutoBool.Auto, new PropertyChangedCallback(Axis.OnShowMinorTickMarksPropertyChanged)));
        public static readonly DependencyProperty ShowScrollZoomBarProperty = DependencyProperty.Register("ShowScrollZoomBar", typeof(bool), typeof(Axis), new PropertyMetadata(false, new PropertyChangedCallback(Axis.OnShowScrollZoomBarPropertyChanged)));
        public static readonly DependencyProperty ActualShowScrollZoomBarProperty = DependencyProperty.Register("ActualShowScrollZoomBar", typeof(bool), typeof(Axis), new PropertyMetadata(false, new PropertyChangedCallback(Axis.OnActualShowScrollZoomBarPropertyChanged)));
        public static readonly DependencyProperty LabelAngleProperty = DependencyProperty.Register("LabelAngle", typeof(double?), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnLabelAnglePropertyChanged)));
        public static readonly DependencyProperty ActualLabelAngleProperty = DependencyProperty.Register("ActualLabelAngle", typeof(double?), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnActualLabelAnglePropertyChanged)));
        public static readonly DependencyProperty CanRotateLabelsProperty = DependencyProperty.Register("CanRotateLabels", typeof(bool), typeof(Axis), new PropertyMetadata(true, new PropertyChangedCallback(Axis.OnCanRotateLabelsPropertyChanged)));
        public static readonly DependencyProperty CanWordWrapLabelsProperty = DependencyProperty.Register("CanWordWrapLabels", typeof(bool), typeof(Axis), new PropertyMetadata(true, new PropertyChangedCallback(Axis.OnCanWordWrapLabelsPropertyChanged)));
        public static readonly DependencyProperty CanStaggerLabelsProperty = DependencyProperty.Register("CanStaggerLabels", typeof(bool), typeof(Axis), new PropertyMetadata(true, new PropertyChangedCallback(Axis.OnCanStaggerLabelsPropertyChanged)));
        public static readonly DependencyProperty CanHideLabelsProperty = DependencyProperty.Register("CanHideLabels", typeof(bool), typeof(Axis), new PropertyMetadata(true, new PropertyChangedCallback(Axis.OnCanHideLabelsPropertyChanged)));
        public static readonly DependencyProperty UseDefaultSizeProperty = DependencyProperty.Register("UseDefaultSize", typeof(bool), typeof(Axis), new PropertyMetadata(false));
        internal const string ScalePropertyName = "Scale";
        internal const string LineStylePropertyName = "LineStyle";
        internal const string TitlePropertyName = "Title";
        internal const string ActualDisplayUnitTitlePropertyName = "ActualDisplayUnitTitle";
        internal const string OrientationPropertyName = "Orientation";
        internal const string PositionPropertyName = "Position";
        internal const string IsReversedPropertyName = "IsReversed";
        internal const string IsScrollZoomBarAllowsZoomingPropertyName = "IsScrollZoomBarAllowsZooming";
        internal const string ActualIsZoomEnabledPropertyName = "ActualIsZoomEnabled";
        internal const string IsScrollZoomBarAllwaysMaximizedPropertyName = "IsScrollZoomBarAllwaysMaximized";
        internal const string IsAllowsAutoZoomPropertyName = "IsAllowsAutoZoom";
        internal const string ViewSizeInPercentPropertyName = "ViewSizeInPercent";
        internal const string ViewPositionInPercentPropertyName = "ViewPositionInPercent";
        internal const string IsMarginVisiblePropertyName = "IsMarginVisible";
        internal const string TitleStylePropertyName = "TitleStyle";
        internal const string DisplayUnitTitleStylePropertyName = "DisplayUnitTitleStyle";
        internal const string StrokeDashTypePropertyName = "StrokeDashType";
        internal const string MajorGridlineStylePropertyName = "MajorGridlineStyle";
        internal const string MajorTickMarkStylePropertyName = "MajorTickMarkStyle";
        internal const string MinorGridlineStylePropertyName = "MinorGridlineStyle";
        internal const string MinorTickMarkStylePropertyName = "MinorTickMarkStyle";
        internal const string LabelStylePropertyName = "LabelStyle";
        internal const string ScrollZoomBarStylePropertyName = "ScrollZoomBarStyle";
        internal const string ShowMajorGridlinesPropertyName = "ShowMajorGridlines";
        internal const string ShowMajorTickMarksPropertyName = "ShowMajorTickMarks";
        internal const string ShowLabelsPropertyName = "ShowLabels";
        internal const string ShowTitlesPropertyName = "ShowTitles";
        internal const string ShowAxisLinePropertyName = "ShowAxisLine";
        internal const string ShowMinorGridlinesPropertyName = "ShowMinorGridlines";
        internal const string ShowMinorTickMarksPropertyName = "ShowMinorTickMarks";
        internal const string ShowScrollZoomBarPropertyName = "ShowScrollZoomBar";
        internal const string ActualShowScrollZoomBarPropertyName = "ActualShowScrollZoomBar";
        internal const string LabelAnglePropertyName = "LabelAngle";
        public const string ActualLabelAnglePropertyName = "ActualLabelAngle";
        internal const string CanRotateLabelsPropertyName = "CanRotateLabels";
        internal const string CanWordWrapLabelsPropertyName = "CanWordWrapLabels";
        internal const string CanStaggerLabelsPropertyName = "CanStaggerLabels";
        internal const string CanHideLabelsPropertyName = "CanHideLabels";
        private AxisPresenter _axisPresenter;
        private ChartArea _chartArea;

        public Scale Scale
        {
            get
            {
                return this.GetValue(Axis.ScaleProperty) as Scale;
            }
            set
            {
                this.SetValue(Axis.ScaleProperty, value);
            }
        }

        public Style LineStyle
        {
            get
            {
                return this.GetValue(Axis.LineStyleProperty) as Style;
            }
            set
            {
                this.SetValue(Axis.LineStyleProperty, value);
            }
        }

        public object Title
        {
            get
            {
                return this.GetValue(Axis.TitleProperty);
            }
            set
            {
                this.SetValue(Axis.TitleProperty, value);
            }
        }

        public object ActualDisplayUnitTitle
        {
            get
            {
                return this.GetValue(Axis.ActualDisplayUnitTitleProperty);
            }
            private set
            {
                this.SetValue(Axis.ActualDisplayUnitTitleProperty, value);
            }
        }

        public AxisOrientation Orientation
        {
            get
            {
                return (AxisOrientation)this.GetValue(Axis.OrientationProperty);
            }
            set
            {
                this.SetValue(Axis.OrientationProperty, value);
            }
        }

        public AxisPosition Position
        {
            get
            {
                return (AxisPosition)this.GetValue(Axis.PositionProperty);
            }
            set
            {
                this.SetValue(Axis.PositionProperty, value);
            }
        }

        public bool IsReversed
        {
            get
            {
                return (bool)this.GetValue(Axis.IsReversedProperty);
            }
            set
            {
                this.SetValue(Axis.IsReversedProperty, value);
            }
        }

        public bool IsScrollZoomBarAllowsZooming
        {
            get
            {
                return (bool)this.GetValue(Axis.IsScrollZoomBarAllowsZoomingProperty);
            }
            set
            {
                this.SetValue(Axis.IsScrollZoomBarAllowsZoomingProperty, value);
            }
        }

        public bool ActualIsZoomEnabled
        {
            get
            {
                return (bool)this.GetValue(Axis.ActualIsZoomEnabledProperty);
            }
            private set
            {
                this.SetValue(Axis.ActualIsZoomEnabledProperty, value);
            }
        }

        public bool IsScrollZoomBarAllwaysMaximized
        {
            get
            {
                return (bool)this.GetValue(Axis.IsScrollZoomBarAllwaysMaximizedProperty);
            }
            set
            {
                this.SetValue(Axis.IsScrollZoomBarAllwaysMaximizedProperty, value);
            }
        }

        public bool IsAllowsAutoZoom
        {
            get
            {
                return (bool)this.GetValue(Axis.IsAllowsAutoZoomProperty);
            }
            set
            {
                this.SetValue(Axis.IsAllowsAutoZoomProperty, value);
            }
        }

        internal bool ScheduleOneScaleUpdate { get; set; }

        public double ViewSizeInPercent
        {
            get
            {
                return (double)this.GetValue(Axis.ViewSizeInPercentProperty);
            }
            set
            {
                this.SetValue(Axis.ViewSizeInPercentProperty, value);
            }
        }

        public double ViewPositionInPercent
        {
            get
            {
                return (double)this.GetValue(Axis.ViewPositionInPercentProperty);
            }
            set
            {
                this.SetValue(Axis.ViewPositionInPercentProperty, value);
            }
        }

        public AutoBool IsMarginVisible
        {
            get
            {
                return (AutoBool)this.GetValue(Axis.IsMarginVisibleProperty);
            }
            set
            {
                this.SetValue(Axis.IsMarginVisibleProperty, value);
            }
        }

        public Style TitleStyle
        {
            get
            {
                return this.GetValue(Axis.TitleStyleProperty) as Style;
            }
            set
            {
                this.SetValue(Axis.TitleStyleProperty, value);
            }
        }

        public Style DisplayUnitTitleStyle
        {
            get
            {
                return this.GetValue(Axis.DisplayUnitTitleStyleProperty) as Style;
            }
            set
            {
                this.SetValue(Axis.DisplayUnitTitleStyleProperty, value);
            }
        }

        public StrokeDashType StrokeDashType
        {
            get
            {
                return (StrokeDashType)this.GetValue(Axis.StrokeDashTypeProperty);
            }
            set
            {
                this.SetValue(Axis.StrokeDashTypeProperty, value);
            }
        }

        public Style MajorGridlineStyle
        {
            get
            {
                return (Style)this.GetValue(Axis.MajorGridlineStyleProperty);
            }
            set
            {
                this.SetValue(Axis.MajorGridlineStyleProperty, value);
            }
        }

        public Style MajorTickMarkStyle
        {
            get
            {
                return (Style)this.GetValue(Axis.MajorTickMarkStyleProperty);
            }
            set
            {
                this.SetValue(Axis.MajorTickMarkStyleProperty, value);
            }
        }

        public Style MinorGridlineStyle
        {
            get
            {
                return (Style)this.GetValue(Axis.MinorGridlineStyleProperty);
            }
            set
            {
                this.SetValue(Axis.MinorGridlineStyleProperty, value);
            }
        }

        public Style MinorTickMarkStyle
        {
            get
            {
                return (Style)this.GetValue(Axis.MinorTickMarkStyleProperty);
            }
            set
            {
                this.SetValue(Axis.MinorTickMarkStyleProperty, value);
            }
        }

        public Style LabelStyle
        {
            get
            {
                return (Style)this.GetValue(Axis.LabelStyleProperty);
            }
            set
            {
                this.SetValue(Axis.LabelStyleProperty, value);
            }
        }

        public Style ScrollZoomBarStyle
        {
            get
            {
                return (Style)this.GetValue(Axis.ScrollZoomBarStyleProperty);
            }
            set
            {
                this.SetValue(Axis.ScrollZoomBarStyleProperty, value);
            }
        }

        public bool ShowMajorGridlines
        {
            get
            {
                return (bool)this.GetValue(Axis.ShowMajorGridlinesProperty);
            }
            set
            {
                this.SetValue(Axis.ShowMajorGridlinesProperty, value);
            }
        }

        public bool ShowMajorTickMarks
        {
            get
            {
                return (bool)this.GetValue(Axis.ShowMajorTickMarksProperty);
            }
            set
            {
                this.SetValue(Axis.ShowMajorTickMarksProperty, value);
            }
        }

        public bool ShowLabels
        {
            get
            {
                return (bool)this.GetValue(Axis.ShowLabelsProperty);
            }
            set
            {
                this.SetValue(Axis.ShowLabelsProperty, value);
            }
        }

        public bool ShowTitles
        {
            get
            {
                return (bool)this.GetValue(Axis.ShowTitlesProperty);
            }
            set
            {
                this.SetValue(Axis.ShowTitlesProperty, value);
            }
        }

        public bool ShowAxisLine
        {
            get
            {
                return (bool)this.GetValue(Axis.ShowAxisLineProperty);
            }
            set
            {
                this.SetValue(Axis.ShowAxisLineProperty, value);
            }
        }

        public AutoBool ShowMinorGridlines
        {
            get
            {
                return (AutoBool)this.GetValue(Axis.ShowMinorGridlinesProperty);
            }
            set
            {
                this.SetValue(Axis.ShowMinorGridlinesProperty, value);
            }
        }

        public AutoBool ShowMinorTickMarks
        {
            get
            {
                return (AutoBool)this.GetValue(Axis.ShowMinorTickMarksProperty);
            }
            set
            {
                this.SetValue(Axis.ShowMinorTickMarksProperty, value);
            }
        }

        public bool ShowScrollZoomBar
        {
            get
            {
                return (bool)this.GetValue(Axis.ShowScrollZoomBarProperty);
            }
            set
            {
                this.SetValue(Axis.ShowScrollZoomBarProperty, value);
            }
        }

        public bool ActualShowScrollZoomBar
        {
            get
            {
                return (bool)this.GetValue(Axis.ActualShowScrollZoomBarProperty);
            }
            private set
            {
                this.SetValue(Axis.ActualShowScrollZoomBarProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<double>))]
        public double? LabelAngle
        {
            get
            {
                return (double?)this.GetValue(Axis.LabelAngleProperty);
            }
            set
            {
                this.SetValue(Axis.LabelAngleProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<double>))]
        public double? ActualLabelAngle
        {
            get
            {
                return (double?)this.GetValue(Axis.ActualLabelAngleProperty);
            }
            internal set
            {
                this.SetValue(Axis.ActualLabelAngleProperty, value);
            }
        }

        public bool CanRotateLabels
        {
            get
            {
                return (bool)this.GetValue(Axis.CanRotateLabelsProperty);
            }
            set
            {
                this.SetValue(Axis.CanRotateLabelsProperty, value);
            }
        }

        public bool CanWordWrapLabels
        {
            get
            {
                return (bool)this.GetValue(Axis.CanWordWrapLabelsProperty);
            }
            set
            {
                this.SetValue(Axis.CanWordWrapLabelsProperty, value);
            }
        }

        public bool CanStaggerLabels
        {
            get
            {
                return (bool)this.GetValue(Axis.CanStaggerLabelsProperty);
            }
            set
            {
                this.SetValue(Axis.CanStaggerLabelsProperty, value);
            }
        }

        public bool CanHideLabels
        {
            get
            {
                return (bool)this.GetValue(Axis.CanHideLabelsProperty);
            }
            set
            {
                this.SetValue(Axis.CanHideLabelsProperty, value);
            }
        }

        public bool IsAutoCreated { get; private set; }

        public bool UseDefaultSize
        {
            get
            {
                return (bool)this.GetValue(Axis.UseDefaultSizeProperty);
            }
            set
            {
                this.SetValue(Axis.UseDefaultSizeProperty, value);
            }
        }

        internal AxisPresenter AxisPresenter
        {
            get
            {
                if (this._axisPresenter == null)
                    this._axisPresenter = this.CreateAxisPresenter();
                return this._axisPresenter;
            }
        }

        public ChartArea ChartArea
        {
            get
            {
                return this._chartArea;
            }
            internal set
            {
                if (this._chartArea == value)
                    return;
                ChartArea oldValue = this._chartArea;
                this._chartArea = value;
                this.OnChartAreaPropertyChanged(oldValue, this._chartArea);
            }
        }

        public IUpdatable Parent
        {
            get
            {
                return null;
            }
        }

        AxisPresenter IAxisPresenterProvider.AxisPresenter
        {
            get
            {
                return this.AxisPresenter;
            }
        }

        public event EventHandler ScaleChanged;

        public event EventHandler<ScaleViewChangedArgs> ScaleViewChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public Axis()
        {
            this.DefaultStyleKey = typeof(Axis);
        }

        private static void OnScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnScalePropertyChanged((Scale)e.OldValue, (Scale)e.NewValue);
        }

        protected virtual void OnScalePropertyChanged(Scale oldValue, Scale newValue)
        {
            if (oldValue != null)
            {
                oldValue.Updated -= new EventHandler(this.Scale_Updated);
                oldValue.ViewChanged -= new EventHandler<ScaleViewChangedArgs>(this.Scale_ViewChanged);
                oldValue.ElementChanged -= new EventHandler(this.Scale_ElementChanged);
            }
            if (newValue != null)
            {
                newValue.Updated += new EventHandler(this.Scale_Updated);
                newValue.ViewChanged += new EventHandler<ScaleViewChangedArgs>(this.Scale_ViewChanged);
                newValue.ElementChanged += new EventHandler(this.Scale_ElementChanged);
            }
            this.UpdateActualDisplayUnit();
            this.OnPropertyChanged("Scale");
        }

        private void Scale_ViewChanged(object sender, ScaleViewChangedArgs e)
        {
            if (!(e.OldRange != e.NewRange))
                return;
            this.OnScaleViewChanged(e.OldRange, e.NewRange);
        }

        private void Scale_Updated(object sender, EventArgs e)
        {
            this.OnScaleChanged();
            this.InvalidateMeasure();
            this.Update();
        }

        private void Scale_ElementChanged(object sender, EventArgs e)
        {
            this.InvalidateMeasure();
            this.Update();
        }

        private void OnScaleChanged()
        {
            this.UpdateActualDisplayUnit();
            EventHandler eventHandler = this.ScaleChanged;
            if (eventHandler == null)
                return;
            eventHandler(this, EventArgs.Empty);
        }

        private void OnScaleViewChanged(Range<IComparable> oldRange, Range<IComparable> newRange)
        {
            if (this.ScaleViewChanged == null)
                return;
            this.ScaleViewChanged(this, new ScaleViewChangedArgs()
            {
                NewRange = newRange,
                OldRange = oldRange
            });
        }

        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis axis = (Axis)d;
            object oldValue = e.OldValue;
            object newValue = e.NewValue;
            if (oldValue == newValue)
                return;
            axis.OnTitlePropertyChanged(oldValue, newValue);
        }

        protected virtual void OnTitlePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("Title");
        }

        private static void OnActualDisplayUnitTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis axis = (Axis)d;
            object oldValue = e.OldValue;
            object newValue = e.NewValue;
            if (oldValue == newValue)
                return;
            axis.OnActualDisplayUnitTitlePropertyChanged(oldValue, newValue);
        }

        protected virtual void OnActualDisplayUnitTitlePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ActualDisplayUnitTitle");
        }

        private void UpdateActualDisplayUnit()
        {
            NumericScale numericScale = this.Scale as NumericScale;
            if (numericScale != null && numericScale.DisplayUnitSystem != null)
                this.ActualDisplayUnitTitle = numericScale.DisplayUnitSystem.ActualTitle;
            else
                this.ActualDisplayUnitTitle = null;
        }

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis axis = (Axis)d;
            AxisOrientation oldValue = (AxisOrientation)e.OldValue;
            AxisOrientation newValue = (AxisOrientation)e.NewValue;
            if (oldValue == newValue)
                return;
            axis.OnOrientationPropertyChanged(oldValue, newValue);
        }

        protected virtual void OnOrientationPropertyChanged(AxisOrientation oldValue, AxisOrientation newValue)
        {
            this.OnPropertyChanged("Orientation");
        }

        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis axis = (Axis)d;
            AxisOrientation oldValue = (AxisOrientation)e.OldValue;
            AxisOrientation newValue = (AxisOrientation)e.NewValue;
            if (newValue == oldValue)
                return;
            axis.OnPositionPropertyChanged(oldValue, newValue);
        }

        protected virtual void OnPositionPropertyChanged(AxisOrientation oldValue, AxisOrientation newValue)
        {
            this.OnPropertyChanged("Position");
        }

        private static void OnIsReversedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnIsReversedPropertyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnIsReversedPropertyChanged(bool oldValue, bool newValue)
        {
            this.OnPropertyChanged("IsReversed");
        }

        private static void OnIsScrollZoomBarAllowsZoomingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnIsScrollZoomBarAllowsZoomingPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsScrollZoomBarAllowsZoomingPropertyChanged(object oldValue, object newValue)
        {
            this.UpdateActualProperties();
            this.OnPropertyChanged("IsScrollZoomBarAllowsZooming");
        }

        private static void OnActualIsZoomEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnActualIsZoomEnabledPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualIsZoomEnabledPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ActualIsZoomEnabled");
        }

        private static void OnIsScrollZoomBarAllwaysMaximizedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnIsScrollZoomBarAllwaysMaximizedPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsScrollZoomBarAllwaysMaximizedPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("IsScrollZoomBarAllwaysMaximized");
        }

        private static void OnIsAllowsAutoZoomPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnIsAllowsAutoZoomPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsAllowsAutoZoomPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("IsAllowsAutoZoom");
        }

        private static void OnViewSizeInPercentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnViewSizeInPercentPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnViewSizeInPercentPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ViewSizeInPercent");
            this.UpdateActualProperties();
        }

        private static void OnViewPositionInPercentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnViewPositionInPercentPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnViewPositionInPercentPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ViewPositionInPercent");
        }

        private static void OnIsMarginVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnIsMarginVisiblePropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsMarginVisiblePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("IsMarginVisible");
        }

        private static void OnTitleStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)o).OnTitleStyleChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnTitleStyleChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("TitleStyle");
        }

        private static void OnDisplayUnitTitleStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)o).OnDisplayUnitTitleStyleChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnDisplayUnitTitleStyleChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("DisplayUnitTitleStyle");
        }

        private static void OnStrokeDashTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnStrokeDashTypePropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnStrokeDashTypePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("StrokeDashType");
        }

        private static void OnMajorGridlineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis axis = (Axis)d;
            object oldValue = e.OldValue;
            object newValue = e.NewValue;
            if (oldValue == newValue)
                return;
            axis.OnMajorGridlineStylePropertyChanged(oldValue, newValue);
        }

        protected virtual void OnMajorGridlineStylePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("MajorGridlineStyle");
        }

        private static void OnMajorTickMarkStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis axis = (Axis)d;
            object oldValue = e.OldValue;
            object newValue = e.NewValue;
            if (oldValue == newValue)
                return;
            axis.OnMajorTickMarkStylePropertyChanged(oldValue, newValue);
        }

        protected virtual void OnMajorTickMarkStylePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("MajorTickMarkStyle");
        }

        private static void OnMinorGridlineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis axis = (Axis)d;
            object oldValue = e.OldValue;
            object newValue = e.NewValue;
            if (oldValue == newValue)
                return;
            axis.OnMinorGridlineStylePropertyChanged(oldValue, newValue);
        }

        protected virtual void OnMinorGridlineStylePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("MinorGridlineStyle");
        }

        private static void OnMinorTickMarkStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis axis = (Axis)d;
            object oldValue = e.OldValue;
            object newValue = e.NewValue;
            if (oldValue == newValue)
                return;
            axis.OnMinorTickMarkStylePropertyChanged(oldValue, newValue);
        }

        protected virtual void OnMinorTickMarkStylePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("MinorTickMarkStyle");
        }

        private static void OnLabelStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnLabelStylePropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnLabelStylePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("LabelStyle");
        }

        private static void OnScrollZoomBarStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnScrollZoomBarStylePropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnScrollZoomBarStylePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ScrollZoomBarStyle");
        }

        private static void OnShowMajorGridlinesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnShowMajorGridlinesPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnShowMajorGridlinesPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ShowMajorGridlines");
        }

        private static void OnShowMajorTickMarksPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnShowMajorTickMarksPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnShowMajorTickMarksPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ShowMajorTickMarks");
        }

        private static void OnShowLabelsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnShowLabelsPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnShowLabelsPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ShowLabels");
        }

        private static void OnShowTitlesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnShowTitlesPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnShowTitlesPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ShowTitles");
        }

        private static void OnShowAxisLinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnShowAxisLinePropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnShowAxisLinePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ShowAxisLine");
        }

        private static void OnShowMinorGridlinesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnShowMinorGridlinesPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnShowMinorGridlinesPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ShowMinorGridlines");
        }

        private static void OnShowMinorTickMarksPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnShowMinorTickMarksPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnShowMinorTickMarksPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ShowMinorTickMarks");
        }

        private static void OnShowScrollZoomBarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnShowScrollZoomBarPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnShowScrollZoomBarPropertyChanged(object oldValue, object newValue)
        {
            this.UpdateActualProperties();
            this.OnPropertyChanged("ShowScrollZoomBar");
        }

        private static void OnActualShowScrollZoomBarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnActualShowScrollZoomBarPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualShowScrollZoomBarPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ActualShowScrollZoomBar");
        }

        private static void OnLabelAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnLabelAnglePropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnLabelAnglePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("LabelAngle");
        }

        private static void OnActualLabelAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnActualLabelAnglePropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualLabelAnglePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ActualLabelAngle");
        }

        private static void OnCanRotateLabelsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnCanRotateLabelsPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnCanRotateLabelsPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("CanRotateLabels");
        }

        private static void OnCanWordWrapLabelsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnCanWordWrapLabelsPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnCanWordWrapLabelsPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("CanWordWrapLabels");
        }

        private static void OnCanStaggerLabelsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnCanStaggerLabelsPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnCanStaggerLabelsPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("CanStaggerLabels");
        }

        private static void OnCanHideLabelsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnCanHideLabelsPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnCanHideLabelsPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("CanHideLabels");
        }

        internal virtual AxisPresenter CreateAxisPresenter()
        {
            return new XYAxisPresenter(this);
        }

        protected virtual void OnChartAreaPropertyChanged(ChartArea oldValue, ChartArea newValue)
        {
            if (newValue != null && oldValue != null)
                throw new InvalidOperationException("ChartAreaPropertyNotNull");
        }

        internal void UpdateActualProperties()
        {
            if (this.IsAllowsAutoZoom)
            {
                if (this.Scale is ICategoryScale)
                {
                    this.ActualIsZoomEnabled = false;
                    this.ActualShowScrollZoomBar = this.AxisPresenter.IsScaleZoomed;
                }
                else
                {
                    this.ActualIsZoomEnabled = true;
                    this.ActualShowScrollZoomBar = this.Scale != null && !this.Scale.IsEmpty && this.Scale.ActualZoomRange.HasData && this.Scale.ActualZoomRange.Maximum > 1.0;
                }
            }
            else
            {
                this.ActualIsZoomEnabled = this.IsScrollZoomBarAllowsZooming;
                this.ActualShowScrollZoomBar = this.ShowScrollZoomBar;
            }
        }

        internal virtual void OnAvailableSizeChanging()
        {
            this.ScheduleOneScaleUpdate = true;
            this.AxisPresenter.ResetAxisLabels();
        }

        protected virtual Range<double> GetScaleRangeOverride()
        {
            return Range<double>.Empty;
        }

        internal Range<double> GetScaleRangeOverrideInternal()
        {
            return this.GetScaleRangeOverride();
        }

        public Range<double> GetScaleRange()
        {
            return this.AxisPresenter.ActualScaleRange;
        }

        public void Update()
        {
            if (this.ChartArea == null)
                return;
            this.AxisPresenter.InvalidateAxis();
            this.UpdateActualProperties();
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged == null)
                return;
            this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        internal static Axis CreateAxis(string name, AxisOrientation orientation)
        {
            Axis axis = new Axis();
            axis.IsAutoCreated = true;
            axis.Name = name;
            axis.Orientation = orientation;
            return axis;
        }
    }
}
