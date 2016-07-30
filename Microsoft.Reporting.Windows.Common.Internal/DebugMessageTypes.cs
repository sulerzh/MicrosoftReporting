using System;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    [Flags]
    public enum DebugMessageTypes
    {
        None = 0,
        UpdateSession = 1,
        Scale = 2,
        AxesPresenters = 4,
        LayoutMeasureArrange = 8,
        ChartArea = 16,
        SeriesPresenters = 32,
        Selection = 64,
        BubbleTraceManager = 128,
    }
}
