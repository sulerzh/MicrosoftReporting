using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class LegendSymbol : Control
    {
        public static readonly DependencyProperty SymbolWidthProperty = DependencyProperty.Register("SymbolWidth", typeof(double?), typeof(LegendSymbol), new PropertyMetadata(null, (d, e) => ((LegendSymbol)d).OnSymbolWidthPropertyChanged((double?)e.OldValue, (double?)e.NewValue)));
        public static readonly DependencyProperty ActualSymbolWidthProperty = DependencyProperty.Register("ActualSymbolWidth", typeof(double), typeof(LegendSymbol), new PropertyMetadata(16.0, (d, e) => ((LegendSymbol)d).OnActualSymbolWidthPropertyChanged((double)e.OldValue, (double)e.NewValue)));
        public static readonly DependencyProperty SymbolHeightProperty = DependencyProperty.Register("SymbolHeight", typeof(double?), typeof(LegendSymbol), new PropertyMetadata(null, (d, e) => ((LegendSymbol)d).OnSymbolHeightPropertyChanged((double?)e.OldValue, (double?)e.NewValue)));
        public static readonly DependencyProperty ActualSymbolHeightProperty = DependencyProperty.Register("ActualSymbolHeight", typeof(double), typeof(LegendSymbol), new PropertyMetadata(10.0, (d, e) => ((LegendSymbol)d).OnActualSymbolHeightPropertyChanged((double)e.OldValue, (double)e.NewValue)));
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(LegendSymbol), new PropertyMetadata(null, (d, e) => ((LegendSymbol)d).OnFillPropertyChanged((Brush)e.OldValue, (Brush)e.NewValue)));
        public static readonly DependencyProperty SymbolBorderBrushProperty = DependencyProperty.Register("SymbolBorderBrush", typeof(Brush), typeof(LegendSymbol), new PropertyMetadata(null, (d, e) => ((LegendSymbol)d).OnSymbolBorderBrushPropertyChanged((Brush)e.OldValue, (Brush)e.NewValue)));
        public static readonly DependencyProperty ActualSymbolBorderBrushProperty = DependencyProperty.Register("ActualSymbolBorderBrush", typeof(Brush), typeof(LegendSymbol), new PropertyMetadata(LegendSymbol.Defaults.SymbolBorderBrush, (d, e) => ((LegendSymbol)d).OnActualSymbolBorderBrushPropertyChanged((Brush)e.OldValue, (Brush)e.NewValue)));
        public static readonly DependencyProperty SymbolBorderThicknessProperty = DependencyProperty.Register("SymbolBorderThickness", typeof(double), typeof(LegendSymbol), new PropertyMetadata(1.0, (d, e) => ((LegendSymbol)d).OnSymbolBorderThicknessPropertyChanged((double)e.OldValue, (double)e.NewValue)));
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(LegendSymbol), new PropertyMetadata(null, (d, e) => ((LegendSymbol)d).OnStrokePropertyChanged((Brush)e.OldValue, (Brush)e.NewValue)));
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(LegendSymbol), new PropertyMetadata(3.0, (d, e) => ((LegendSymbol)d).OnStrokeThicknessPropertyChanged((double)e.OldValue, (double)e.NewValue)));
        public static readonly DependencyProperty MarkerProperty = DependencyProperty.Register("Marker", typeof(MarkerType), typeof(LegendSymbol), new PropertyMetadata(MarkerType.None, (d, e) => ((LegendSymbol)d).OnMarkerPropertyChanged((MarkerType)e.OldValue, (MarkerType)e.NewValue)));
        public static readonly DependencyProperty MarkerSizeProperty = DependencyProperty.Register("MarkerSize", typeof(double), typeof(LegendSymbol), new PropertyMetadata(10.0, (d, e) => ((LegendSymbol)d).OnMarkerSizePropertyChanged((double)e.OldValue, (double)e.NewValue)));
        public static readonly DependencyProperty MarkerFillProperty = DependencyProperty.Register("MarkerFill", typeof(Brush), typeof(LegendSymbol), new PropertyMetadata(LegendSymbol.Defaults.MarkerFill, (d, e) => ((LegendSymbol)d).OnMarkerFillPropertyChanged((Brush)e.OldValue, (Brush)e.NewValue)));
        public static readonly DependencyProperty MarkerStrokeProperty = DependencyProperty.Register("MarkerStroke", typeof(Brush), typeof(LegendSymbol), new PropertyMetadata(LegendSymbol.Defaults.MarkerStroke, (d, e) => ((LegendSymbol)d).OnMarkerStrokePropertyChanged((Brush)e.OldValue, (Brush)e.NewValue)));
        public static readonly DependencyProperty MarkerStrokeThicknessProperty = DependencyProperty.Register("MarkerStrokeThickness", typeof(double), typeof(LegendSymbol), new PropertyMetadata(1.0, (d, e) => ((LegendSymbol)d).OnMarkerStrokeThicknessPropertyChanged((double)e.OldValue, (double)e.NewValue)));
        internal const string SymbolWidthPropertyName = "SymbolWidth";
        internal const string ActualSymbolWidthPropertyName = "ActualSymbolWidth";
        internal const string SymbolHeightPropertyName = "SymbolHeight";
        internal const string ActualSymbolHeightPropertyName = "ActualSymbolHeight";
        internal const string FillPropertyName = "Fill";
        internal const string SymbolBorderBrushPropertyName = "SymbolBorderBrush";
        internal const string ActualSymbolBorderBrushPropertyName = "ActualSymbolBorderBrush";
        internal const string SymbolBorderThicknessPropertyName = "SymbolBorderThickness";
        internal const string StrokePropertyName = "Stroke";
        internal const string StrokeThicknessPropertyName = "StrokeThickness";
        internal const string MarkerPropertyName = "Marker";
        internal const string MarkerSizePropertyName = "MarkerSize";
        internal const string MarkerFillPropertyName = "MarkerFill";
        internal const string MarkerStrokePropertyName = "MarkerStroke";
        internal const string MarkerStrokeThicknessPropertyName = "MarkerStrokeThickness";

        public object Owner { get; set; }

        public double? SymbolWidth
        {
            get
            {
                return (double?)this.GetValue(LegendSymbol.SymbolWidthProperty);
            }
            set
            {
                this.SetValue(LegendSymbol.SymbolWidthProperty, value);
            }
        }

        [DefaultValue(16.0)]
        public double ActualSymbolWidth
        {
            get
            {
                return (double)this.GetValue(LegendSymbol.ActualSymbolWidthProperty);
            }
            set
            {
                this.SetValue(LegendSymbol.ActualSymbolWidthProperty, value);
            }
        }

        public double? SymbolHeight
        {
            get
            {
                return (double?)this.GetValue(LegendSymbol.SymbolHeightProperty);
            }
            set
            {
                this.SetValue(LegendSymbol.SymbolHeightProperty, value);
            }
        }

        [DefaultValue(10.0)]
        public double ActualSymbolHeight
        {
            get
            {
                return (double)this.GetValue(LegendSymbol.ActualSymbolHeightProperty);
            }
            set
            {
                this.SetValue(LegendSymbol.ActualSymbolHeightProperty, value);
            }
        }

        public Brush Fill
        {
            get
            {
                return this.GetValue(LegendSymbol.FillProperty) as Brush;
            }
            set
            {
                this.SetValue(LegendSymbol.FillProperty, value);
            }
        }

        public Brush SymbolBorderBrush
        {
            get
            {
                return this.GetValue(LegendSymbol.SymbolBorderBrushProperty) as Brush;
            }
            set
            {
                this.SetValue(LegendSymbol.SymbolBorderBrushProperty, value);
            }
        }

        [DefaultValue(typeof(SolidColorBrush), "Black")]
        public Brush ActualSymbolBorderBrush
        {
            get
            {
                return this.GetValue(LegendSymbol.ActualSymbolBorderBrushProperty) as Brush;
            }
            set
            {
                this.SetValue(LegendSymbol.ActualSymbolBorderBrushProperty, value);
            }
        }

        [DefaultValue(1.0)]
        public double SymbolBorderThickness
        {
            get
            {
                return (double)this.GetValue(LegendSymbol.SymbolBorderThicknessProperty);
            }
            set
            {
                this.SetValue(LegendSymbol.SymbolBorderThicknessProperty, value);
            }
        }

        public Brush Stroke
        {
            get
            {
                return this.GetValue(LegendSymbol.StrokeProperty) as Brush;
            }
            set
            {
                this.SetValue(LegendSymbol.StrokeProperty, value);
            }
        }

        [DefaultValue(3.0)]
        public double StrokeThickness
        {
            get
            {
                return (double)this.GetValue(LegendSymbol.StrokeThicknessProperty);
            }
            set
            {
                this.SetValue(LegendSymbol.StrokeThicknessProperty, value);
            }
        }

        [DefaultValue(MarkerType.None)]
        public MarkerType Marker
        {
            get
            {
                return (MarkerType)this.GetValue(LegendSymbol.MarkerProperty);
            }
            set
            {
                this.SetValue(LegendSymbol.MarkerProperty, value);
            }
        }

        [DefaultValue(10.0)]
        public double MarkerSize
        {
            get
            {
                return (double)this.GetValue(LegendSymbol.MarkerSizeProperty);
            }
            set
            {
                this.SetValue(LegendSymbol.MarkerSizeProperty, value);
            }
        }

        public Brush MarkerFill
        {
            get
            {
                return this.GetValue(LegendSymbol.MarkerFillProperty) as Brush;
            }
            set
            {
                this.SetValue(LegendSymbol.MarkerFillProperty, value);
            }
        }

        public Brush MarkerStroke
        {
            get
            {
                return this.GetValue(LegendSymbol.MarkerStrokeProperty) as Brush;
            }
            set
            {
                this.SetValue(LegendSymbol.MarkerStrokeProperty, value);
            }
        }

        [DefaultValue(1.0)]
        public double MarkerStrokeThickness
        {
            get
            {
                return (double)this.GetValue(LegendSymbol.MarkerStrokeThicknessProperty);
            }
            set
            {
                this.SetValue(LegendSymbol.MarkerStrokeThicknessProperty, value);
            }
        }

        internal Path FillPath { get; set; }

        internal Path StrokePath { get; set; }

        internal Path MarkerPath { get; set; }

        public LegendSymbol()
        {
            this.DefaultStyleKey = typeof(LegendSymbol);
            this.UpdateAutoProperties();
        }

        private void OnSymbolWidthPropertyChanged(double? oldValue, double? newValue)
        {
            if (newValue.HasValue)
                this.ActualSymbolWidth = newValue.Value;
            else
                this.ActualSymbolWidth = this.DetermineDefaultSymbolSize().Width;
        }

        private void OnActualSymbolWidthPropertyChanged(double oldValue, double newValue)
        {
        }

        private void OnSymbolHeightPropertyChanged(double? oldValue, double? newValue)
        {
            if (newValue.HasValue)
                this.ActualSymbolHeight = newValue.Value;
            else
                this.ActualSymbolHeight = this.DetermineDefaultSymbolSize().Height;
        }

        private void OnActualSymbolHeightPropertyChanged(double oldValue, double newValue)
        {
        }

        private void OnFillPropertyChanged(Brush oldValue, Brush newValue)
        {
            this.UpdateAutoProperties();
        }

        private void OnSymbolBorderBrushPropertyChanged(Brush oldValue, Brush newValue)
        {
            if (newValue != null)
                this.ActualSymbolBorderBrush = newValue;
            else if (this.Fill != null)
                this.ActualSymbolBorderBrush = LegendSymbol.Defaults.SymbolBorderBrush;
            else
                this.ActualSymbolBorderBrush = null;
        }

        private void OnActualSymbolBorderBrushPropertyChanged(Brush oldValue, Brush newValue)
        {
        }

        private void OnSymbolBorderThicknessPropertyChanged(double oldValue, double newValue)
        {
        }

        private void OnStrokePropertyChanged(Brush oldValue, Brush newValue)
        {
            this.UpdateAutoProperties();
        }

        private void OnStrokeThicknessPropertyChanged(double oldValue, double newValue)
        {
            this.UpdateAutoProperties();
        }

        private void OnMarkerPropertyChanged(MarkerType oldValue, MarkerType newValue)
        {
            this.UpdateAutoProperties();
        }

        private void OnMarkerSizePropertyChanged(double oldValue, double newValue)
        {
            this.UpdateAutoProperties();
        }

        private void OnMarkerFillPropertyChanged(Brush oldValue, Brush newValue)
        {
        }

        private void OnMarkerStrokePropertyChanged(Brush oldValue, Brush newValue)
        {
        }

        private void OnMarkerStrokeThicknessPropertyChanged(double oldValue, double newValue)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.FillPath = this.GetTemplateChild("FillPath") as Path;
            this.StrokePath = this.GetTemplateChild("StrokePath") as Path;
            this.MarkerPath = this.GetTemplateChild("MarkerPath") as Path;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            Geometry[] geometryArray = this.BuildGeometries();
            this.FillPath.Data = geometryArray[0];
            this.StrokePath.Data = geometryArray[1];
            this.MarkerPath.Data = geometryArray[2];
            return size;
        }

        private RectangleGeometry BuildFillGeometry(bool halfFill)
        {
            return new RectangleGeometry() { Rect = !halfFill ? new Rect(0.0, 0.0, this.ActualSymbolWidth, this.ActualSymbolHeight) : new Rect(0.0, this.ActualSymbolHeight / 2.0, this.ActualSymbolWidth, this.ActualSymbolHeight / 2.0) };
        }

        private LineGeometry BuildStrokeGeometry()
        {
            return new LineGeometry() { StartPoint = new Point(0.0, this.ActualSymbolHeight / 2.0), EndPoint = new Point(this.ActualSymbolWidth, this.ActualSymbolHeight / 2.0) };
        }

        private Geometry[] BuildGeometries()
        {
            Geometry[] geometryArray = new Geometry[3];
            if (this.Fill != null)
                geometryArray[0] = this.BuildFillGeometry(this.Stroke != null);
            if (this.Stroke != null)
                geometryArray[1] = this.BuildStrokeGeometry();
            if (this.Marker != MarkerType.None)
                geometryArray[2] = VisualUtilities.GetMarkerGeometry(this.Marker, new Point(this.MarkerSize / 2.0 - this.ActualSymbolWidth / 2.0, this.MarkerSize / 2.0 - this.ActualSymbolHeight / 2.0), new Size(this.MarkerSize, this.MarkerSize), 0.0, 0.0);
            return geometryArray;
        }

        internal Size DetermineDefaultSymbolSize()
        {
            Size size = new Size(0.0, 0.0);
            if (this.Fill != null)
                size = new Size(16.0, 10.0);
            if (this.Stroke != null)
            {
                size.Height = Math.Max(size.Height, this.StrokeThickness + 6.0);
                size.Width = Math.Max(size.Width, size.Height * 5.0);
            }
            if (this.Marker != MarkerType.None)
            {
                size.Height = Math.Max(size.Height, this.MarkerSize + 6.0);
                size.Width = Math.Max(size.Width, this.MarkerSize + 6.0);
            }
            size.Width = Math.Max(size.Width, this.MinWidth);
            return size;
        }

        internal double GetDesiredSymbolWidth()
        {
            if (this.SymbolWidth.HasValue)
                return this.SymbolWidth.Value;
            return this.DetermineDefaultSymbolSize().Width;
        }

        private void UpdateAutoProperties()
        {
            this.OnSymbolBorderBrushPropertyChanged(this.SymbolBorderBrush, this.SymbolBorderBrush);
            this.OnSymbolWidthPropertyChanged(this.SymbolWidth, this.SymbolWidth);
            this.OnSymbolHeightPropertyChanged(this.SymbolHeight, this.SymbolHeight);
        }

        internal static class Defaults
        {
            private static Brush _symbolBorderBrush = new SolidColorBrush(Colors.Black);
            private static Brush _markerStroke = new SolidColorBrush(Colors.Black);
            private static Brush _markerFill = new SolidColorBrush(Colors.Red);
            internal const double ActualSymbolWidth = 16.0;
            internal const double ActualSymbolHeight = 10.0;
            internal const MarkerType Marker = MarkerType.None;
            internal const double MarkerSize = 10.0;
            internal const double StrokeThickness = 3.0;
            internal const double MarkerStrokeThickness = 1.0;
            internal const double SymbolBorderThickness = 1.0;

            internal static Brush SymbolBorderBrush
            {
                get
                {
                    return LegendSymbol.Defaults._symbolBorderBrush;
                }
            }

            internal static Brush MarkerStroke
            {
                get
                {
                    return LegendSymbol.Defaults._markerStroke;
                }
            }

            internal static Brush MarkerFill
            {
                get
                {
                    return LegendSymbol.Defaults._markerFill;
                }
            }
        }
    }
}
