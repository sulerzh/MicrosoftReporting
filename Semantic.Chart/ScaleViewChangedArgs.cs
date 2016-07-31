using Microsoft.Reporting.Windows.Common.Internal;
using System;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class ScaleViewChangedArgs : EventArgs
    {
        public Range<IComparable> OldRange { get; set; }

        public Range<IComparable> NewRange { get; set; }
    }
}
