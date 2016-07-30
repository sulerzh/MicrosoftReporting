using System;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class ValueChangedEventArgs : EventArgs
    {
        public string ValueName { get; private set; }

        public object OldValue { get; private set; }

        public object NewValue { get; private set; }

        public ValueChangedEventArgs(string valueName, object oldValue, object newValue)
        {
            this.ValueName = valueName;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }
}
