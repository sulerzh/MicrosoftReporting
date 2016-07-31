using System;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class TreeChangedEventArgs : EventArgs
    {
        public object Source { get; private set; }

        public EventArgs InnerEventArgs { get; private set; }

        public TreeChangedEventArgs(object source, EventArgs innerEventArgs)
        {
            this.Source = source;
            this.InnerEventArgs = innerEventArgs;
        }
    }
}
