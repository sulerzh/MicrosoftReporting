using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public abstract class ScaleElementDefinition : DependencyObject, INotifyPropertyChanged
    {
        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register("Visibility", typeof(Visibility), typeof(ScaleElementDefinition), new PropertyMetadata(new PropertyChangedCallback(ScaleElementDefinition.OnVisibilityChanged)));
        public static readonly DependencyProperty GroupProperty = DependencyProperty.Register("Group", typeof(ScaleElementGroup), typeof(ScaleElementDefinition), new PropertyMetadata(new PropertyChangedCallback(ScaleElementDefinition.OnGroupChanged)));
        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level", typeof(int), typeof(ScaleElementDefinition), new PropertyMetadata(0, new PropertyChangedCallback(ScaleElementDefinition.OnLevelChanged)));
        private const string VisibilityPropertyName = "Visibility";
        private const string GroupPropertyName = "Group";
        private const string LevelPropertyName = "Level";

        internal Scale Scale { get; set; }

        internal ScaleElementKind Kind { get; set; }

        public Visibility Visibility
        {
            get
            {
                return (Visibility)this.GetValue(ScaleElementDefinition.VisibilityProperty);
            }
            set
            {
                this.SetValue(ScaleElementDefinition.VisibilityProperty, value);
            }
        }

        public ScaleElementGroup Group
        {
            get
            {
                return (ScaleElementGroup)this.GetValue(ScaleElementDefinition.GroupProperty);
            }
            set
            {
                this.SetValue(ScaleElementDefinition.GroupProperty, value);
            }
        }

        public int Level
        {
            get
            {
                return (int)this.GetValue(ScaleElementDefinition.LevelProperty);
            }
            set
            {
                this.SetValue(ScaleElementDefinition.LevelProperty, value);
            }
        }

        public IEnumerable<ScalePosition> Positions { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScaleElementDefinition)d).OnVisibilityChanged((Visibility)e.OldValue, (Visibility)e.NewValue);
        }

        protected virtual void OnVisibilityChanged(Visibility oldValue, Visibility newValue)
        {
            this.OnPropertyChanged("Visibility");
        }

        private static void OnGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScaleElementDefinition)d).OnGroupChanged((ScaleElementGroup)e.OldValue, (ScaleElementGroup)e.NewValue);
        }

        protected virtual void OnGroupChanged(ScaleElementGroup oldValue, ScaleElementGroup newValue)
        {
            this.OnPropertyChanged("Group");
        }

        private static void OnLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScaleElementDefinition)d).OnLevelChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual void OnLevelChanged(int oldValue, int newValue)
        {
            this.OnPropertyChanged("Level");
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged == null)
                return;
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
