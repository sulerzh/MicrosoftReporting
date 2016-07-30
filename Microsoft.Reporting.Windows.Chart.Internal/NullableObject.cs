namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public struct NullableObject<T> where T : class
    {
        public T Value;
        public bool HasValue;

        public NullableObject(T value)
        {
            this.Value = value;
            this.HasValue = true;
        }
    }
}
