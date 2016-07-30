using System;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public interface INotifyTreeChanged
    {
        event EventHandler<TreeChangedEventArgs> TreeChanged;
    }
}
