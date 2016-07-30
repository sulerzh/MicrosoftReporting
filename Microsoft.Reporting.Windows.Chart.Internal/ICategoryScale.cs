namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public interface ICategoryScale
    {
        double ActualMinimum { get; }

        double ActualMaximum { get; }

        void ZoomToValue(double viewMinimum, double viewMaximum);
    }
}
