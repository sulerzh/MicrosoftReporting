using Microsoft.Reporting.Windows.Common.Internal;
using System;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public struct ScaleDefaults
    {
        public AutoBool IncludeZero { get; private set; }

        public double MaxAllowedMargin { get; private set; }

        public ScaleDefaults(AutoBool includeZero, double maxAllowedMargin)
        {
            this = new ScaleDefaults();
            this.IncludeZero = includeZero;
            this.MaxAllowedMargin = maxAllowedMargin;
        }

        public static ScaleDefaults operator +(ScaleDefaults value, ScaleDefaults other)
        {
            return new ScaleDefaults(ValueHelper.Or(value.IncludeZero, other.IncludeZero), Math.Max(value.MaxAllowedMargin, other.MaxAllowedMargin));
        }

        public override bool Equals(object obj)
        {
            ScaleDefaults scaleDefaults = (ScaleDefaults)obj;
            if (scaleDefaults.MaxAllowedMargin == this.MaxAllowedMargin)
                return scaleDefaults.IncludeZero == this.IncludeZero;
            return false;
        }

        public override int GetHashCode()
        {
            return this.MaxAllowedMargin.GetHashCode() ^ this.IncludeZero.GetHashCode();
        }
    }
}
