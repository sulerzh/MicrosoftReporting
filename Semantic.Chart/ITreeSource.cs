namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal interface ITreeSource
    {
        object GetRoot();

        int GetChildCount(object parent);

        object GetChild(object parent, int index);

        object GetParent(object child);

        int GetChildIndex(object parent, object child);

        bool IsLeaf(object item);

        int GetLeafCount();

        object GetLeaf(int leafIndex);
    }
}
