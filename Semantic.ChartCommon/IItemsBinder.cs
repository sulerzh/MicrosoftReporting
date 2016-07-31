namespace Microsoft.Reporting.Windows.Common.Internal
{
    public interface IItemsBinder<T>
    {
        void Bind(T target, object source);

        void Unbind(T target, object source);
    }
}
