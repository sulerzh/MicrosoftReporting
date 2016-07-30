using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public interface ISelectableView
    {
        Geometry GetSelectionGeometry();
    }
}
