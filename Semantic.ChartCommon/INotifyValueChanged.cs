using System.ComponentModel;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public interface INotifyValueChanged : INotifyPropertyChanged
    {
        event ValueChangedEventHandler ValueChanged;
    }
}
