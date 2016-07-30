using System;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    [Flags]
    public enum ContentPositions
    {
        None = 0,
        TopLeft = 1,
        TopCenter = 2,
        TopRight = 4,
        MiddleLeft = 8,
        MiddleCenter = 16,
        MiddleRight = 32,
        BottomLeft = 64,
        BottomCenter = 128,
        BottomRight = 256,
        InsideCenter = 512,
        InsideBase = 1024,
        InsideEnd = 2048,
        OutsideBase = 4096,
        OutsideEnd = 8192,
        All = OutsideEnd | OutsideBase | InsideEnd | InsideBase | InsideCenter | BottomRight | BottomCenter | BottomLeft | MiddleRight | MiddleCenter | MiddleLeft | TopRight | TopCenter | TopLeft,
    }
}
