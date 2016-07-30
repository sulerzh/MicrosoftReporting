using System;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class PropertyAnimator : DependencyObject
    {
        public static readonly DependencyProperty AnimatedValueProperty = DependencyProperty.Register("AnimatedValue", typeof(object), typeof(PropertyAnimator), new PropertyMetadata(new PropertyChangedCallback(PropertyAnimator.OnAnimatedValueChanged)));
        internal const string AnimatedValuePropertyName = "AnimatedValue";
        private Action<object, object> _updateAction;

        public Action<object, object> UpdateAction
        {
            get
            {
                return this._updateAction;
            }
            set
            {
                this._updateAction = value;
            }
        }

        public object AnimatedValue
        {
            get
            {
                return this.GetValue(PropertyAnimator.AnimatedValueProperty);
            }
            set
            {
                this.SetValue(PropertyAnimator.AnimatedValueProperty, value);
            }
        }

        private static void OnAnimatedValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (((PropertyAnimator)o).UpdateAction == null)
                return;
            ((PropertyAnimator)o).UpdateAction(e.OldValue, e.NewValue);
        }
    }
}
