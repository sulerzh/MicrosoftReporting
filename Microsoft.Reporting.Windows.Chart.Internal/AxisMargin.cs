namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal struct AxisMargin
    {
        public static readonly AxisMargin Empty = new AxisMargin(0.0, 0.0);
        private double _start;
        private double _end;

        public double Start
        {
            get
            {
                return this._start;
            }
        }

        public double End
        {
            get
            {
                return this._end;
            }
        }

        public AxisMargin(double start, double end)
        {
            this._start = start;
            this._end = end;
        }

        public AxisMargin Extend(AxisMargin value)
        {
            if (value.Start > this._start)
                this._start = value.Start;
            if (value.End > this._end)
                this._end = value.End;
            return new AxisMargin(this._start, this._end);
        }
    }
}
