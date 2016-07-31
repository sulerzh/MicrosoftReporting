using System;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    [Flags]
    public enum LabelAutoFit
    {
        None = 0,
        Stagger = 1,
        RotateWithStep30 = 2,
        RotateWithStep90 = 4,
        WordWrap = 8,
        All = WordWrap | RotateWithStep30 | Stagger,
    }
}
