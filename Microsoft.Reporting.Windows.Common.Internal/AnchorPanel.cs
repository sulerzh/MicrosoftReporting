using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class AnchorPanel : Panel
    {
        public static readonly DependencyProperty CollisionDetectionEnabledProperty = DependencyProperty.Register("CollisionDetectionEnabled", typeof(bool), typeof(AnchorPanel), new PropertyMetadata(false, new PropertyChangedCallback(AnchorPanel.OnCollisionDetectionEnabledChanged)));
        public static readonly DependencyProperty MovingStepProperty = DependencyProperty.Register("MovingStep", typeof(double), typeof(AnchorPanel), new PropertyMetadata(3.0, new PropertyChangedCallback(AnchorPanel.OnMovingStepChanged)));
        public static readonly DependencyProperty AnchorPointProperty = DependencyProperty.RegisterAttached("AnchorPoint", typeof(Point), typeof(AnchorPanel), new PropertyMetadata(new Point(double.PositiveInfinity, double.PositiveInfinity), new PropertyChangedCallback(AnchorPanel.OnAnchorPointPropertyChanged)));
        public static readonly DependencyProperty AnchorMarginProperty = DependencyProperty.RegisterAttached("AnchorMargin", typeof(double), typeof(AnchorPanel), new PropertyMetadata(0.0, new PropertyChangedCallback(AnchorPanel.OnAnchorMarginPropertyChanged)));
        public static readonly DependencyProperty ContentPositionProperty = DependencyProperty.RegisterAttached("ContentPosition", typeof(ContentPositions), typeof(AnchorPanel), new PropertyMetadata(ContentPositions.TopCenter, new PropertyChangedCallback(AnchorPanel.OnContentPositionPropertyChanged)));
        public static readonly DependencyProperty AnchorRectProperty = DependencyProperty.RegisterAttached("AnchorRect", typeof(Rect), typeof(AnchorPanel), new PropertyMetadata(Rect.Empty, new PropertyChangedCallback(AnchorPanel.OnAnchorRectPropertyChanged)));
        public static readonly DependencyProperty AnchorRectOrientationProperty = DependencyProperty.RegisterAttached("AnchorRectOrientation", typeof(RectOrientation), typeof(AnchorPanel), new PropertyMetadata(RectOrientation.None, new PropertyChangedCallback(AnchorPanel.OnAnchorRectOrientationPropertyChanged)));
        public static readonly DependencyProperty RepositionOverlappedProperty = DependencyProperty.RegisterAttached("RepositionOverlapped", typeof(bool), typeof(AnchorPanel), new PropertyMetadata(true, new PropertyChangedCallback(AnchorPanel.OnRepositionOverlappedPropertyChanged)));
        public static readonly DependencyProperty HideOverlappedProperty = DependencyProperty.RegisterAttached("HideOverlapped", typeof(bool), typeof(AnchorPanel), new PropertyMetadata(true, new PropertyChangedCallback(AnchorPanel.OnHideOverlappedPropertyChanged)));
        public static readonly DependencyProperty OutsidePlacementProperty = DependencyProperty.RegisterAttached("OutsidePlacement", typeof(OutsidePlacement), typeof(AnchorPanel), new PropertyMetadata(OutsidePlacement.Partial, new PropertyChangedCallback(AnchorPanel.OnOutsidePlacementPropertyChanged)));
        public static readonly DependencyProperty ValidContentPositionsProperty = DependencyProperty.RegisterAttached("ValidContentPositions", typeof(ContentPositions), typeof(AnchorPanel), new PropertyMetadata(ContentPositions.None, new PropertyChangedCallback(AnchorPanel.OnValidContentPositionsPropertyChanged)));
        public static readonly DependencyProperty MinimumMovingDistanceProperty = DependencyProperty.RegisterAttached("MinimumMovingDistance", typeof(double), typeof(AnchorPanel), new PropertyMetadata(0.0, new PropertyChangedCallback(AnchorPanel.OnMinimumMovingDistancePropertyChanged)));
        public static readonly DependencyProperty MaximumMovingDistanceProperty = DependencyProperty.RegisterAttached("MaximumMovingDistance", typeof(double), typeof(AnchorPanel), new PropertyMetadata(30.0, new PropertyChangedCallback(AnchorPanel.OnMaximumMovingDistancePropertyChanged)));
        internal const double MaxPixelDistance = 1000.0;
        private const double HorizontalSpacing = 1.0;
        private bool _isMeasurePending;
        private bool _isMeasureCompleted;

        public bool CollisionDetectionEnabled
        {
            get
            {
                return (bool)this.GetValue(AnchorPanel.CollisionDetectionEnabledProperty);
            }
            set
            {
                this.SetValue(AnchorPanel.CollisionDetectionEnabledProperty, value);
            }
        }

        public double MovingStep
        {
            get
            {
                return (double)this.GetValue(AnchorPanel.MovingStepProperty);
            }
            set
            {
                this.SetValue(AnchorPanel.MovingStepProperty, value);
            }
        }

        internal int CallNestLevelFromRoot { get; set; }

        private static void OnCollisionDetectionEnabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AnchorPanel.InvalidateAnchorPanel(o as AnchorPanel);
        }

        private static void OnMovingStepChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            double newValue = (double)e.NewValue;
            double oldValue = (double)e.OldValue;
            ((AnchorPanel)o).OnMovingStepChanged(oldValue, newValue);
        }

        private void OnMovingStepChanged(double oldValue, double newValue)
        {
            if (this.CallNestLevelFromRoot != 0)
                return;
            if (newValue >= 1.0 && newValue <= 1000.0)
            {
                AnchorPanel.InvalidateAnchorPanel(this);
            }
            else
            {
                ++this.CallNestLevelFromRoot;
                this.MovingStep = oldValue;
                --this.CallNestLevelFromRoot;
                throw new ArgumentOutOfRangeException(Properties.Resources.AnchorPanel_MovingStep_ValueMustBeInRange1To1000);
            }
        }

        public static Point GetAnchorPoint(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (Point)element.GetValue(AnchorPanel.AnchorPointProperty);
        }

        public static void SetAnchorPoint(UIElement element, Point point)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(AnchorPanel.AnchorPointProperty, point);
        }

        private static void OnAnchorPointPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnchorPanel.InvalidateAnchorPanel(d);
        }

        public static double GetAnchorMargin(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (double)element.GetValue(AnchorPanel.AnchorMarginProperty);
        }

        public static void SetAnchorMargin(UIElement element, double margin)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(AnchorPanel.AnchorMarginProperty, margin);
        }

        private static void OnAnchorMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double num = (double)e.NewValue;
            AnchorPanel panel = VisualTreeHelper.GetParent(d) as AnchorPanel;
            if (panel == null || panel.CallNestLevelFromRoot != 0)
                return;
            if (num >= 0.0 && num <= 1000.0)
            {
                AnchorPanel.InvalidateAnchorPanel(panel);
            }
            else
            {
                ++panel.CallNestLevelFromRoot;
                d.SetValue(e.Property, e.OldValue);
                --panel.CallNestLevelFromRoot;
                throw new ArgumentOutOfRangeException(Properties.Resources.AnchorPanel_AnchorMargin_ValueMustBeInRange0To1000);
            }
        }

        public static ContentPositions GetContentPosition(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (ContentPositions)element.GetValue(AnchorPanel.ContentPositionProperty);
        }

        public static void SetContentPosition(UIElement element, ContentPositions alignment)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(AnchorPanel.ContentPositionProperty, alignment);
        }

        private static void OnContentPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnchorPanel.InvalidateAnchorPanel(d);
        }

        public static Rect GetAnchorRect(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (Rect)element.GetValue(AnchorPanel.AnchorRectProperty);
        }

        public static void SetAnchorRect(UIElement element, Rect rect)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(AnchorPanel.AnchorRectProperty, rect);
        }

        private static void OnAnchorRectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnchorPanel.InvalidateAnchorPanel(d);
        }

        public static RectOrientation GetAnchorRectOrientation(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (RectOrientation)element.GetValue(AnchorPanel.AnchorRectOrientationProperty);
        }

        public static void SetAnchorRectOrientation(UIElement element, RectOrientation anchorRectOrientation)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(AnchorPanel.AnchorRectOrientationProperty, anchorRectOrientation);
        }

        private static void OnAnchorRectOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnchorPanel.InvalidateAnchorPanel(d);
        }

        public static bool GetRepositionOverlapped(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (bool)element.GetValue(AnchorPanel.RepositionOverlappedProperty);
        }

        public static void SetRepositionOverlapped(UIElement element, bool repositionOverlapped)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(AnchorPanel.RepositionOverlappedProperty, repositionOverlapped);
        }

        private static void OnRepositionOverlappedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnchorPanel.InvalidateAnchorPanel(d);
        }

        public static bool GetHideOverlapped(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (bool)element.GetValue(AnchorPanel.HideOverlappedProperty);
        }

        public static void SetHideOverlapped(UIElement element, bool hideOverlapped)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(AnchorPanel.HideOverlappedProperty, hideOverlapped);
        }

        private static void OnHideOverlappedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnchorPanel.InvalidateAnchorPanel(d);
        }

        public static OutsidePlacement GetOutsidePlacement(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (OutsidePlacement)element.GetValue(AnchorPanel.OutsidePlacementProperty);
        }

        public static void SetOutsidePlacement(UIElement element, OutsidePlacement outsidePlacement)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(AnchorPanel.OutsidePlacementProperty, outsidePlacement);
        }

        private static void OnOutsidePlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnchorPanel.InvalidateAnchorPanel(d);
        }

        public static ContentPositions GetValidContentPositions(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (ContentPositions)element.GetValue(AnchorPanel.ValidContentPositionsProperty);
        }

        public static void SetValidContentPositions(UIElement element, ContentPositions validContentPositions)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(AnchorPanel.ValidContentPositionsProperty, validContentPositions);
        }

        private static void OnValidContentPositionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnchorPanel.InvalidateAnchorPanel(d);
        }

        public static double GetMinimumMovingDistance(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (double)element.GetValue(AnchorPanel.MinimumMovingDistanceProperty);
        }

        public static void SetMinimumMovingDistance(UIElement element, double minimumMovingDistance)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(AnchorPanel.MinimumMovingDistanceProperty, minimumMovingDistance);
        }

        private static void OnMinimumMovingDistancePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double num = (double)e.NewValue;
            AnchorPanel panel = VisualTreeHelper.GetParent(d) as AnchorPanel;
            if (panel == null || panel.CallNestLevelFromRoot != 0)
                return;
            if (num >= 0.0 && num <= 1000.0)
            {
                AnchorPanel.InvalidateAnchorPanel(panel);
            }
            else
            {
                ++panel.CallNestLevelFromRoot;
                d.SetValue(e.Property, e.OldValue);
                --panel.CallNestLevelFromRoot;
                throw new ArgumentOutOfRangeException(Properties.Resources.AnchorPanel_MinimumMovingDistance_ValueMustBeInRange0To1000);
            }
        }

        public static double GetMaximumMovingDistance(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (double)element.GetValue(AnchorPanel.MaximumMovingDistanceProperty);
        }

        public static void SetMaximumMovingDistance(UIElement element, double maximumMovingDistance)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(AnchorPanel.MaximumMovingDistanceProperty, maximumMovingDistance);
        }

        private static void OnMaximumMovingDistancePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double num = (double)e.NewValue;
            AnchorPanel panel = VisualTreeHelper.GetParent(d) as AnchorPanel;
            if (panel == null || panel.CallNestLevelFromRoot != 0)
                return;
            if (num >= 0.0 && num <= 1000.0)
            {
                AnchorPanel.InvalidateAnchorPanel(panel);
            }
            else
            {
                ++panel.CallNestLevelFromRoot;
                d.SetValue(e.Property, e.OldValue);
                --panel.CallNestLevelFromRoot;
                throw new ArgumentOutOfRangeException(Properties.Resources.AnchorPanel_MaximumMovingDistance_ValueMustBeInRange0To1000);
            }
        }

        private static void InvalidateAnchorPanel(DependencyObject d)
        {
            AnchorPanel.InvalidateAnchorPanel(VisualTreeHelper.GetParent(d) as AnchorPanel);
        }

        private static void InvalidateAnchorPanel(AnchorPanel panel)
        {
            if (panel == null)
                return;
            panel.Invalidate();
        }

        public void Invalidate()
        {
            this._isMeasureCompleted = false;
            if (this._isMeasurePending)
                return;
            this._isMeasurePending = true;
            this.InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = base.MeasureOverride(constraint);
            if (this._isMeasurePending)
            {
                this._isMeasurePending = false;
                this.MeasureChildren();
                this._isMeasureCompleted = true;
            }
            return size;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (this.Children.Count == 0 || !this._isMeasureCompleted)
                return arrangeSize;
            this.ArrangeChildren(arrangeSize);
            return arrangeSize;
        }

        private void MeasureChildren()
        {
            Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement child in this.Children)
                child.Measure(availableSize);
        }

        private bool ShouldArrange(UIElement element)
        {
            ContentControl contentControl = element as ContentControl;
            if (contentControl != null && contentControl.Content == null)
                return false;
            Size desiredSize = element.DesiredSize;
            ILabelControl labelControl = element as ILabelControl;
            if (labelControl != null)
                desiredSize = labelControl.GetDesiredSize();
            return desiredSize.Width != 0.0 && desiredSize.Height != 0.0;
        }

        private void ArrangeChildren(Size arrangeSize)
        {
            AnchorPanel.ArrangeGrid arrangeGrid = new AnchorPanel.ArrangeGrid(arrangeSize, this.Children.OfType<UIElement>());
            foreach (UIElement child in this.Children)
            {
                Rect rect = Rect.Empty;
                if (child.Visibility == Visibility.Visible)
                {
                    if (!this.ShouldArrange(child))
                    {
                        ILabelControl labelControl = child as ILabelControl;
                        if (labelControl != null)
                            labelControl.CalloutGeometry = null;
                    }
                    else
                    {
                        AnchoredElement anchoredElement = new AnchoredElement(child, arrangeSize);
                        rect = AnchorPanel.CalculateContentPosition(anchoredElement);
                        rect = this.AdjustPosition(arrangeGrid, anchoredElement, rect);
                        ILabelControl labelControl = child as ILabelControl;
                        if (labelControl != null)
                        {
                            if (anchoredElement.Offset > anchoredElement.AnchorMargin && anchoredElement.ContentPosition != ContentPositions.InsideBase && (anchoredElement.ContentPosition != ContentPositions.InsideCenter && anchoredElement.ContentPosition != ContentPositions.InsideEnd) || (anchoredElement.ContentPosition == ContentPositions.BottomLeft || anchoredElement.ContentPosition == ContentPositions.BottomRight || (anchoredElement.ContentPosition == ContentPositions.TopLeft || anchoredElement.ContentPosition == ContentPositions.TopRight)))
                            {
                                Point calloutPoint = AnchorPanel.GetCalloutPoint(rect, anchoredElement.ContentPosition, anchoredElement.AnchorRectOrientation);
                                labelControl.UpdateCalloutGeometry(new Point(calloutPoint.X - rect.X, calloutPoint.Y - rect.Y), new Point(anchoredElement.AnchorPoint.X - rect.X, anchoredElement.AnchorPoint.Y - rect.Y));
                            }
                            else
                                labelControl.CalloutGeometry = null;
                        }
                        arrangeGrid.Add(child, rect);
                        if (ValueHelper.CanGraph(rect.Left) && ValueHelper.CanGraph(rect.Right) && (ValueHelper.CanGraph(rect.Width) && ValueHelper.CanGraph(rect.Height)))
                            child.Arrange(rect);
                    }
                }
            }
        }

        private Rect AdjustPosition(AnchorPanel.ArrangeGrid arrangeGrid, AnchoredElement anchoredElement, Rect position)
        {
            if (!this.CollisionDetectionEnabled || !AnchorPanel.GetRepositionOverlapped(anchoredElement.UIElement) || !AnchorPanel.CheckPositionForCollision(arrangeGrid, anchoredElement, position))
                return position;
            Rect rect;
            if (anchoredElement.ContentPosition == ContentPositions.InsideEnd || anchoredElement.ContentPosition == ContentPositions.InsideCenter || (anchoredElement.ContentPosition == ContentPositions.InsideBase || anchoredElement.ContentPosition == ContentPositions.OutsideBase) || anchoredElement.ContentPosition == ContentPositions.OutsideEnd)
            {
                rect = this.AdjustAnchorRectPosition(arrangeGrid, anchoredElement, position);
                if (rect == position)
                    rect = this.AdjustAnchoredPosition(arrangeGrid, anchoredElement, position);
            }
            else
            {
                rect = this.AdjustAnchoredPosition(arrangeGrid, anchoredElement, position);
                if (rect == position)
                    rect = this.AdjustAnchorRectPosition(arrangeGrid, anchoredElement, position);
            }
            bool hideOverlapped = AnchorPanel.GetHideOverlapped(anchoredElement.UIElement);
            if (rect == position && hideOverlapped)
                rect = new Rect(double.MinValue, double.MinValue, 0.0, 0.0);
            return rect;
        }

        private Rect AdjustAnchorRectPosition(AnchorPanel.ArrangeGrid arrangeGrid, AnchoredElement anchoredElement, Rect position)
        {
            if (!anchoredElement.AnchorRect.IsEmpty && ValueHelper.CanGraph(anchoredElement.AnchorRect.X) && (ValueHelper.CanGraph(anchoredElement.AnchorRect.Y) && ValueHelper.CanGraph(anchoredElement.AnchorRect.Right)) && ValueHelper.CanGraph(anchoredElement.AnchorRect.Left))
            {
                ContentPositions[] contentPositionsArray;
                switch (anchoredElement.ContentPosition)
                {
                    case ContentPositions.InsideEnd:
                        contentPositionsArray = new ContentPositions[5]
                        {
              ContentPositions.InsideEnd,
              ContentPositions.InsideCenter,
              ContentPositions.InsideBase,
              ContentPositions.OutsideEnd,
              ContentPositions.OutsideBase
                        };
                        break;
                    case ContentPositions.OutsideBase:
                        contentPositionsArray = new ContentPositions[5]
                        {
              ContentPositions.OutsideBase,
              ContentPositions.InsideBase,
              ContentPositions.InsideCenter,
              ContentPositions.InsideEnd,
              ContentPositions.OutsideEnd
                        };
                        break;
                    case ContentPositions.OutsideEnd:
                        contentPositionsArray = new ContentPositions[5]
                        {
              ContentPositions.OutsideEnd,
              ContentPositions.InsideEnd,
              ContentPositions.InsideCenter,
              ContentPositions.InsideBase,
              ContentPositions.OutsideBase
                        };
                        break;
                    case ContentPositions.InsideCenter:
                        contentPositionsArray = new ContentPositions[5]
                        {
              ContentPositions.InsideCenter,
              ContentPositions.InsideEnd,
              ContentPositions.InsideBase,
              ContentPositions.OutsideEnd,
              ContentPositions.OutsideBase
                        };
                        break;
                    case ContentPositions.InsideBase:
                        contentPositionsArray = new ContentPositions[5]
                        {
              ContentPositions.InsideBase,
              ContentPositions.InsideCenter,
              ContentPositions.InsideEnd,
              ContentPositions.OutsideBase,
              ContentPositions.OutsideEnd
                        };
                        break;
                    default:
                        contentPositionsArray = new ContentPositions[6]
                        {
              anchoredElement.ContentPosition,
              ContentPositions.TopCenter,
              ContentPositions.BottomCenter,
              ContentPositions.MiddleRight,
              ContentPositions.MiddleLeft,
              ContentPositions.MiddleCenter
                        };
                        break;
                }
                if (contentPositionsArray != null)
                {
                    Size desiredSize = AnchorPanel.GetDesiredSize(anchoredElement.UIElement);
                    for (int index = 0; index < contentPositionsArray.Length; ++index)
                    {
                        if ((anchoredElement.ValidContentPositions & contentPositionsArray[index]) == contentPositionsArray[index])
                        {
                            double val1 = anchoredElement.MaximumMovingDistance;
                            double num1 = Math.Max(anchoredElement.MinimumMovingDistance, anchoredElement.AnchorMargin);
                            bool flag = false;
                            switch (contentPositionsArray[index])
                            {
                                case ContentPositions.InsideCenter:
                                    num1 = anchoredElement.MinimumMovingDistance;
                                    flag = true;
                                    val1 = anchoredElement.AnchorRectOrientation == RectOrientation.LeftRight || anchoredElement.AnchorRectOrientation == RectOrientation.RightLeft ? (anchoredElement.AnchorRect.Width - desiredSize.Width) / 2.0 : (anchoredElement.AnchorRect.Height - AnchorPanel.GetDesiredSize(anchoredElement.UIElement).Height) / 2.0;
                                    break;
                                case ContentPositions.InsideBase:
                                case ContentPositions.InsideEnd:
                                    val1 = anchoredElement.AnchorRectOrientation == RectOrientation.LeftRight || anchoredElement.AnchorRectOrientation == RectOrientation.RightLeft ? anchoredElement.AnchorRect.Width - AnchorPanel.GetDesiredSize(anchoredElement.UIElement).Width : anchoredElement.AnchorRect.Height - AnchorPanel.GetDesiredSize(anchoredElement.UIElement).Height;
                                    break;
                            }
                            double num2 = Math.Min(val1, anchoredElement.MaximumMovingDistance);
                            double offset = num1;
                            while (offset.LessOrEqualWithPrecision(num2))
                            {
                                Rect contentPosition1 = AnchorPanel.CalculateContentPosition(anchoredElement, contentPositionsArray[index], desiredSize, offset);
                                if (!AnchorPanel.CheckPositionForCollision(arrangeGrid, anchoredElement, contentPosition1))
                                {
                                    anchoredElement.Offset = offset;
                                    anchoredElement.ContentPosition = contentPositionsArray[index];
                                    return contentPosition1;
                                }
                                if (flag)
                                {
                                    Rect contentPosition2 = AnchorPanel.CalculateContentPosition(anchoredElement, contentPositionsArray[index], desiredSize, -offset);
                                    if (!AnchorPanel.CheckPositionForCollision(arrangeGrid, anchoredElement, contentPosition2))
                                    {
                                        anchoredElement.Offset = offset;
                                        anchoredElement.ContentPosition = contentPositionsArray[index];
                                        return contentPosition2;
                                    }
                                }
                                offset += this.MovingStep;
                            }
                        }
                    }
                }
            }
            return position;
        }

        private Rect AdjustAnchoredPosition(AnchorPanel.ArrangeGrid arrangeGrid, AnchoredElement anchoredElement, Rect position)
        {
            if (ValueHelper.CanGraph(anchoredElement.AnchorPoint.X) && ValueHelper.CanGraph(anchoredElement.AnchorPoint.Y))
            {
                ContentPositions[] contentPositionsArray = new ContentPositions[9] { ContentPositions.BottomCenter, ContentPositions.TopCenter, ContentPositions.MiddleRight, ContentPositions.MiddleLeft, ContentPositions.BottomRight, ContentPositions.BottomLeft, ContentPositions.TopRight, ContentPositions.TopLeft, ContentPositions.MiddleCenter };
                Size desiredSize = AnchorPanel.GetDesiredSize(anchoredElement.UIElement);
                double offset = Math.Max(anchoredElement.MinimumMovingDistance, anchoredElement.AnchorMargin);
                while (offset.LessOrEqualWithPrecision(anchoredElement.MaximumMovingDistance))
                {
                    for (int index = 0; index < contentPositionsArray.Length; ++index)
                    {
                        if ((contentPositionsArray[index] != ContentPositions.MiddleCenter || offset == anchoredElement.MinimumMovingDistance) && (anchoredElement.ValidContentPositions & contentPositionsArray[index]) == contentPositionsArray[index])
                        {
                            Rect contentPosition = AnchorPanel.CalculateContentPosition(anchoredElement, contentPositionsArray[index], desiredSize, offset);
                            if (!AnchorPanel.CheckPositionForCollision(arrangeGrid, anchoredElement, contentPosition))
                            {
                                anchoredElement.Offset = offset;
                                anchoredElement.ContentPosition = contentPositionsArray[index];
                                return contentPosition;
                            }
                        }
                    }
                    offset += this.MovingStep;
                }
            }
            return position;
        }

        private static Rect CalculateContentPosition(AnchoredElement anchoredElement)
        {
            return AnchorPanel.CalculateContentPosition(anchoredElement, anchoredElement.ContentPosition, AnchorPanel.GetDesiredSize(anchoredElement.UIElement), anchoredElement.AnchorMargin);
        }

        private static Size GetDesiredSize(UIElement element)
        {
            Size desiredSize = element.DesiredSize;
            ILabelControl labelControl = element as ILabelControl;
            if (labelControl != null)
            {
                desiredSize = labelControl.GetDesiredSize();
                ++desiredSize.Width;
            }
            return desiredSize;
        }

        private static Rect CalculateContentPosition(AnchoredElement anchoredElement, ContentPositions contentPosition, Size contentSize, double offset)
        {
            if (contentPosition == ContentPositions.InsideEnd || contentPosition == ContentPositions.InsideCenter || (contentPosition == ContentPositions.InsideBase || contentPosition == ContentPositions.OutsideBase) || contentPosition == ContentPositions.OutsideEnd)
                return AnchorPanel.CalculateContentPosition(anchoredElement.AnchorRect, anchoredElement.AnchorRectOrientation, contentPosition, contentSize, offset);
            return AnchorPanel.CalculateContentPosition(anchoredElement.AnchorPoint, contentPosition, contentSize, offset);
        }

        private static Rect CalculateContentPosition(Rect anchorRect, RectOrientation anchorRectOrientation, ContentPositions contentPosition, Size contentSize, double offset)
        {
            switch (contentPosition)
            {
                case ContentPositions.InsideEnd:
                    switch (anchorRectOrientation)
                    {
                        case RectOrientation.BottomTop:
                            return new Rect(AnchorPanel.GetLocation_TopInside(contentSize, anchorRect, offset), contentSize);
                        case RectOrientation.TopBottom:
                            return new Rect(AnchorPanel.GetLocation_BottomInside(contentSize, anchorRect, offset), contentSize);
                        case RectOrientation.RightLeft:
                            return new Rect(AnchorPanel.GetLocation_LeftInside(contentSize, anchorRect, offset), contentSize);
                        default:
                            return new Rect(AnchorPanel.GetLocation_RightInside(contentSize, anchorRect, offset), contentSize);
                    }
                case ContentPositions.OutsideBase:
                    switch (anchorRectOrientation)
                    {
                        case RectOrientation.BottomTop:
                            return new Rect(AnchorPanel.GetLocation_BottomOutside(contentSize, anchorRect, offset), contentSize);
                        case RectOrientation.TopBottom:
                            return new Rect(AnchorPanel.GetLocation_TopOutside(contentSize, anchorRect, offset), contentSize);
                        case RectOrientation.RightLeft:
                            return new Rect(AnchorPanel.GetLocation_RightOutside(contentSize, anchorRect, offset), contentSize);
                        default:
                            return new Rect(AnchorPanel.GetLocation_LeftOutside(contentSize, anchorRect, offset), contentSize);
                    }
                case ContentPositions.OutsideEnd:
                    switch (anchorRectOrientation)
                    {
                        case RectOrientation.BottomTop:
                            return new Rect(AnchorPanel.GetLocation_TopOutside(contentSize, anchorRect, offset), contentSize);
                        case RectOrientation.TopBottom:
                            return new Rect(AnchorPanel.GetLocation_BottomOutside(contentSize, anchorRect, offset), contentSize);
                        case RectOrientation.RightLeft:
                            return new Rect(AnchorPanel.GetLocation_LeftOutside(contentSize, anchorRect, offset), contentSize);
                        default:
                            return new Rect(AnchorPanel.GetLocation_RightOutside(contentSize, anchorRect, offset), contentSize);
                    }
                case ContentPositions.InsideCenter:
                    switch (anchorRectOrientation)
                    {
                        case RectOrientation.BottomTop:
                        case RectOrientation.TopBottom:
                            return new Rect(AnchorPanel.GetLocation_MiddleVertical(contentSize, anchorRect, offset), contentSize);
                        default:
                            return new Rect(AnchorPanel.GetLocation_MiddleHorizontal(contentSize, anchorRect, offset), contentSize);
                    }
                case ContentPositions.InsideBase:
                    switch (anchorRectOrientation)
                    {
                        case RectOrientation.BottomTop:
                            return new Rect(AnchorPanel.GetLocation_BottomInside(contentSize, anchorRect, offset), contentSize);
                        case RectOrientation.TopBottom:
                            return new Rect(AnchorPanel.GetLocation_TopInside(contentSize, anchorRect, offset), contentSize);
                        case RectOrientation.RightLeft:
                            return new Rect(AnchorPanel.GetLocation_RightInside(contentSize, anchorRect, offset), contentSize);
                        default:
                            return new Rect(AnchorPanel.GetLocation_LeftInside(contentSize, anchorRect, offset), contentSize);
                    }
                default:
                    throw new NotSupportedException("Unsupported ContentPosition.");
            }
        }

        private static Point GetLocation_TopInside(Size contentSize, Rect anchorRect, double offset)
        {
            return new Point(anchorRect.X + anchorRect.Width / 2.0 - contentSize.Width / 2.0, anchorRect.Y + offset);
        }

        private static Point GetLocation_BottomInside(Size contentSize, Rect anchorRect, double offset)
        {
            return new Point(anchorRect.X + anchorRect.Width / 2.0 - contentSize.Width / 2.0, anchorRect.Bottom - contentSize.Height - offset);
        }

        private static Point GetLocation_RightInside(Size contentSize, Rect anchorRect, double offset)
        {
            return new Point(anchorRect.Right - contentSize.Width - offset, anchorRect.Y + anchorRect.Height / 2.0 - contentSize.Height / 2.0);
        }

        private static Point GetLocation_LeftInside(Size contentSize, Rect anchorRect, double offset)
        {
            return new Point(anchorRect.X + offset, anchorRect.Y + anchorRect.Height / 2.0 - contentSize.Height / 2.0);
        }

        private static Point GetLocation_TopOutside(Size contentSize, Rect anchorRect, double offset)
        {
            return new Point(anchorRect.X + anchorRect.Width / 2.0 - contentSize.Width / 2.0, anchorRect.Y - contentSize.Height - offset);
        }

        private static Point GetLocation_BottomOutside(Size contentSize, Rect anchorRect, double offset)
        {
            return new Point(anchorRect.X + anchorRect.Width / 2.0 - contentSize.Width / 2.0, anchorRect.Bottom + offset);
        }

        private static Point GetLocation_RightOutside(Size contentSize, Rect anchorRect, double offset)
        {
            return new Point(anchorRect.Right + offset, anchorRect.Y + anchorRect.Height / 2.0 - contentSize.Height / 2.0);
        }

        private static Point GetLocation_LeftOutside(Size contentSize, Rect anchorRect, double offset)
        {
            return new Point(anchorRect.X - contentSize.Width - offset, anchorRect.Y + anchorRect.Height / 2.0 - contentSize.Height / 2.0);
        }

        private static Point GetLocation_MiddleHorizontal(Size contentSize, Rect anchorRect, double offset)
        {
            return new Point(anchorRect.X + anchorRect.Width / 2.0 - contentSize.Width / 2.0 + offset, anchorRect.Y + anchorRect.Height / 2.0 - contentSize.Height / 2.0);
        }

        private static Point GetLocation_MiddleVertical(Size contentSize, Rect anchorRect, double offset)
        {
            return new Point(anchorRect.X + anchorRect.Width / 2.0 - contentSize.Width / 2.0, anchorRect.Y + anchorRect.Height / 2.0 - contentSize.Height / 2.0 + offset);
        }

        private static Rect CalculateContentPosition(Point anchorPoint, ContentPositions contentPosition, Size contentSize, double offset)
        {
            Rect rect = new Rect(new Point(0.0, 0.0), contentSize);
            if (ValueHelper.CanGraph(anchorPoint.X))
            {
                rect.X = anchorPoint.X;
                switch (contentPosition)
                {
                    case ContentPositions.MiddleCenter:
                    case ContentPositions.BottomCenter:
                    case ContentPositions.TopCenter:
                        rect.X -= contentSize.Width / 2.0;
                        break;
                    case ContentPositions.BottomLeft:
                    case ContentPositions.TopLeft:
                    case ContentPositions.MiddleLeft:
                        rect.X -= contentSize.Width;
                        break;
                }
            }
            if (ValueHelper.CanGraph(anchorPoint.Y))
            {
                rect.Y = anchorPoint.Y;
                switch (contentPosition)
                {
                    case ContentPositions.MiddleCenter:
                    case ContentPositions.MiddleRight:
                    case ContentPositions.MiddleLeft:
                        rect.Y -= contentSize.Height / 2.0;
                        break;
                    case ContentPositions.TopLeft:
                    case ContentPositions.TopCenter:
                    case ContentPositions.TopRight:
                        rect.Y -= contentSize.Height;
                        break;
                }
            }
            if (ValueHelper.CanGraph(offset))
            {
                switch (contentPosition)
                {
                    case ContentPositions.BottomCenter:
                        rect.Y += offset;
                        break;
                    case ContentPositions.BottomRight:
                        rect.X += offset;
                        rect.Y += offset;
                        break;
                    case ContentPositions.MiddleRight:
                        rect.X += offset;
                        break;
                    case ContentPositions.BottomLeft:
                        rect.X -= offset;
                        rect.Y += offset;
                        break;
                    case ContentPositions.TopLeft:
                        rect.X -= offset;
                        rect.Y -= offset;
                        break;
                    case ContentPositions.TopCenter:
                        rect.Y -= offset;
                        break;
                    case ContentPositions.TopRight:
                        rect.X += offset;
                        rect.Y -= offset;
                        break;
                    case ContentPositions.MiddleLeft:
                        rect.X -= offset;
                        break;
                }
            }
            return rect;
        }

        private static bool CheckPositionForCollision(AnchorPanel.ArrangeGrid arrangeGrid, AnchoredElement anchoredElement, Rect position)
        {
            if (arrangeGrid.HasConflict(position))
                return true;
            Rect rect = new Rect(new Point(0.0, 0.0), anchoredElement.ArrangeSize).Expand(5.0, 3.0);
            switch (anchoredElement.OutsidePlacement)
            {
                case OutsidePlacement.Disallowed:
                    rect.Intersect(position);
                    if (!rect.Width.LessWithPrecision(position.Width))
                        return rect.Height.LessWithPrecision(position.Height);
                    return true;
                case OutsidePlacement.Partial:
                    rect.Intersect(position);
                    if (!rect.Width.LessWithPrecision(position.Width / 2.0))
                        return rect.Height.LessWithPrecision(position.Height / 2.0);
                    return true;
                default:
                    return false;
            }
        }

        private static Point GetCalloutPoint(Rect position, ContentPositions contentAlignment, RectOrientation rectOrientation)
        {
            Point point = new Point(position.X, position.Y);
            if (contentAlignment == ContentPositions.MiddleCenter || contentAlignment == ContentPositions.MiddleLeft || contentAlignment == ContentPositions.MiddleRight)
                point.Y += position.Height / 2.0;
            else if (contentAlignment == ContentPositions.TopCenter || contentAlignment == ContentPositions.TopLeft || contentAlignment == ContentPositions.TopRight)
                point.Y = position.Bottom;
            if (contentAlignment == ContentPositions.TopCenter || contentAlignment == ContentPositions.MiddleCenter || contentAlignment == ContentPositions.BottomCenter)
                point.X += position.Width / 2.0;
            else if (contentAlignment == ContentPositions.TopLeft || contentAlignment == ContentPositions.MiddleLeft || contentAlignment == ContentPositions.BottomLeft)
                point.X = position.Right;
            if (contentAlignment == ContentPositions.OutsideBase)
            {
                if (rectOrientation == RectOrientation.BottomTop)
                    point = new Point(position.X + position.Width / 2.0, position.Y);
                else if (rectOrientation == RectOrientation.TopBottom)
                    point = new Point(position.X + position.Width / 2.0, position.Bottom);
                else if (rectOrientation == RectOrientation.LeftRight)
                    point = new Point(position.Right, position.Y + position.Height / 2.0);
                else if (rectOrientation == RectOrientation.RightLeft)
                    point = new Point(position.X, position.Y + position.Height / 2.0);
            }
            else if (contentAlignment == ContentPositions.OutsideEnd)
            {
                if (rectOrientation == RectOrientation.BottomTop)
                    point = new Point(position.X + position.Width / 2.0, position.Bottom);
                else if (rectOrientation == RectOrientation.TopBottom)
                    point = new Point(position.X + position.Width / 2.0, position.Y);
                else if (rectOrientation == RectOrientation.LeftRight)
                    point = new Point(position.X, position.Y + position.Height / 2.0);
                else if (rectOrientation == RectOrientation.RightLeft)
                    point = new Point(position.Right, position.Y + position.Height / 2.0);
            }
            return point;
        }

        internal class ArrangeGrid
        {
            private const int MinCount = 1;
            private const int MaxCount = 100;
            private List<AnchorPanel.ArrangeGrid.ElementInfo>[,] _grid;
            private Size _size;
            private Size _cellSize;
            private int _colCount;
            private int _rowCount;

            public ArrangeGrid(Size size, IEnumerable<UIElement> elements)
            {
                this._size = size;
                if (size.Width == 0.0 || size.Height == 0.0)
                {
                    this._cellSize = size;
                    this._colCount = 0;
                    this._rowCount = 0;
                }
                this._cellSize = new Size(0.0, 0.0);
                foreach (UIElement element in elements)
                {
                    Size desiredSize = AnchorPanel.GetDesiredSize(element);
                    double num1 = desiredSize.Width * 2.0;
                    double num2 = desiredSize.Height * 2.0;
                    if (num1 > this._cellSize.Width)
                        this._cellSize.Width = num1;
                    if (num2 > this._cellSize.Height)
                        this._cellSize.Height = num2;
                }
                if (this._cellSize.Width == 0.0)
                    this._cellSize.Width = size.Width;
                if (this._cellSize.Height == 0.0)
                    this._cellSize.Height = size.Height;
                this._colCount = AnchorPanel.ArrangeGrid.GetCount(this._cellSize.Width, this._size.Width, 1, 100);
                this._rowCount = AnchorPanel.ArrangeGrid.GetCount(this._cellSize.Height, this._size.Height, 1, 100);
                this._cellSize.Width = size.Width / this._colCount;
                this._cellSize.Height = size.Height / this._rowCount;
                this._grid = new List<AnchorPanel.ArrangeGrid.ElementInfo>[this._colCount, this._rowCount];
                for (int index1 = 0; index1 < this._colCount; ++index1)
                {
                    for (int index2 = 0; index2 < this._rowCount; ++index2)
                        this._grid[index1, index2] = new List<AnchorPanel.ArrangeGrid.ElementInfo>();
                }
            }

            public void Add(UIElement element, Rect rect)
            {
                AnchorPanel.ArrangeGrid.IntRect gridIndexRect = this.GetGridIndexRect(rect);
                for (int index1 = gridIndexRect.X; index1 < gridIndexRect.X2; ++index1)
                {
                    for (int index2 = gridIndexRect.Y; index2 < gridIndexRect.Y2; ++index2)
                        this._grid[index1, index2].Add(new AnchorPanel.ArrangeGrid.ElementInfo()
                        {
                            Element = element,
                            Rect = rect
                        });
                }
            }

            public bool HasConflict(Rect rect)
            {
                AnchorPanel.ArrangeGrid.IntRect gridIndexRect = this.GetGridIndexRect(rect);
                for (int index1 = gridIndexRect.X; index1 < gridIndexRect.X2; ++index1)
                {
                    for (int index2 = gridIndexRect.Y; index2 < gridIndexRect.Y2; ++index2)
                    {
                        foreach (AnchorPanel.ArrangeGrid.ElementInfo elementInfo in this._grid[index1, index2])
                        {
                            if (elementInfo.Rect.IntersectsWith(rect))
                                return true;
                        }
                    }
                }
                return false;
            }

            internal AnchorPanel.ArrangeGrid.IntRect GetGridIndexRect(Rect rect)
            {
                return new AnchorPanel.ArrangeGrid.IntRect() { X = AnchorPanel.ArrangeGrid.Restrict((int)Math.Floor(rect.X / this._cellSize.Width), 0, this._colCount), Y = AnchorPanel.ArrangeGrid.Restrict((int)Math.Floor(rect.Y / this._cellSize.Height), 0, this._rowCount), X2 = AnchorPanel.ArrangeGrid.Restrict((int)Math.Ceiling(rect.Right / this._cellSize.Width), 0, this._colCount), Y2 = AnchorPanel.ArrangeGrid.Restrict((int)Math.Ceiling(rect.Bottom / this._cellSize.Height), 0, this._rowCount) };
            }

            private static int GetCount(double step, double length, int minCount, int maxCount)
            {
                return AnchorPanel.ArrangeGrid.Restrict((int)Math.Ceiling(length / step), minCount, maxCount);
            }

            private static int Restrict(int x, int min, int max)
            {
                if (x < min)
                    return min;
                if (x > max)
                    return max;
                return x;
            }

            private class ElementInfo
            {
                public UIElement Element { get; set; }

                public Rect Rect { get; set; }
            }

            internal struct IntRect
            {
                public int X;
                public int Y;
                public int X2;
                public int Y2;

                public override string ToString()
                {
                    return string.Format(CultureInfo.InvariantCulture, "IntRect: ({0},{1}) - ({2},{3})", this.X, this.Y, this.X2, this.Y2);
                }
            }
        }
    }
}
