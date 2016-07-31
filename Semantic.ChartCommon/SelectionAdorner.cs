using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class SelectionAdorner : Control
    {
        public static readonly DependencyProperty OutlineVisibilityProperty = DependencyProperty.Register("OutlineVisibility", typeof(Visibility), typeof(SelectionAdorner), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty PointsVisibilityProperty = DependencyProperty.Register("PointsVisibility", typeof(Visibility), typeof(SelectionAdorner), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty OutlineProperty = DependencyProperty.Register("Outline", typeof(Geometry), typeof(SelectionAdorner), new PropertyMetadata(null, new PropertyChangedCallback(SelectionAdorner.OnOutlineChanged)));
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(ObservableCollectionSupportingInitialization<Point>), typeof(SelectionAdorner), new PropertyMetadata(null));
        internal const string OutlineVisibilityPropertyName = "OutlineVisibility";
        internal const string PointsVisibilityPropertyName = "PointsVisibility";

        public Visibility OutlineVisibility
        {
            get
            {
                return (Visibility)this.GetValue(SelectionAdorner.OutlineVisibilityProperty);
            }
            set
            {
                this.SetValue(SelectionAdorner.OutlineVisibilityProperty, value);
            }
        }

        public Visibility PointsVisibility
        {
            get
            {
                return (Visibility)this.GetValue(SelectionAdorner.PointsVisibilityProperty);
            }
            set
            {
                this.SetValue(SelectionAdorner.PointsVisibilityProperty, value);
            }
        }

        public Geometry Outline
        {
            get
            {
                return (Geometry)this.GetValue(SelectionAdorner.OutlineProperty);
            }
            set
            {
                this.SetValue(SelectionAdorner.OutlineProperty, value);
            }
        }

        public ObservableCollectionSupportingInitialization<Point> Points
        {
            get
            {
                return (ObservableCollectionSupportingInitialization<Point>)this.GetValue(SelectionAdorner.PointsProperty);
            }
            private set
            {
                this.SetValue(SelectionAdorner.PointsProperty, value);
            }
        }

        public SelectionAdorner()
        {
            this.DefaultStyleKey = typeof(SelectionAdorner);
            this.Points = new ObservableCollectionSupportingInitialization<Point>();
        }

        private static void OnOutlineChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as SelectionAdorner).OnOutlineChanged(e.OldValue as Geometry, e.NewValue as Geometry);
        }

        private void OnOutlineChanged(Geometry oldValue, Geometry newValue)
        {
            this.UpdatePoints();
        }

        public void UpdatePoints()
        {
            IEnumerable<Point> points = this.Outline.GetPoints();
            this.Points.BeginInit();
            try
            {
                this.Points.Clear();
                foreach (Point point in points)
                    this.Points.Add(point);
            }
            finally
            {
                this.Points.EndInit();
            }
        }
    }
}
