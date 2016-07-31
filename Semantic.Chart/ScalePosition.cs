namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class ScalePosition
    {
        public object Data { get; set; }

        public double Position { get; set; }

        public double? BucketMin { get; set; }

        public double? BucketMax { get; set; }

        public ScalePosition(object data, double projectedPosition)
        {
            this.Data = data;
            this.Position = projectedPosition;
        }

        public ScalePosition(object data, double projectedPosition, double projectedBucketMin, double projectedBucketMax)
        {
            this.Data = data;
            this.Position = projectedPosition;
            this.BucketMin = new double?(projectedBucketMin);
            this.BucketMax = new double?(projectedBucketMax);
        }
    }
}
