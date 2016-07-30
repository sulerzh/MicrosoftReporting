using System;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public interface IHierarchyVirtualizationHelper
    {
        event EventHandler LeavesChanged;

        int GetLeafCount();

        object GetLeaf(int leafIndex);

        int GetLeafIndex(object item);

        object GetParent(object item);

        int GetChildCount(object item);

        object GetChildAt(object item, int index);

        int GetIndex(object item);
    }
}
