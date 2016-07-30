using System;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class Singleton
    {
        public object Instance { get; set; }

        public Action<object> DisposeAction { get; set; }

        public int ReferenceCounter { get; set; }
    }
}
