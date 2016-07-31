using System.Windows.Controls.Primitives;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    internal interface IUnparentedPopupProvider
    {
        Popup UnparentedPopup { get; }
    }
}
