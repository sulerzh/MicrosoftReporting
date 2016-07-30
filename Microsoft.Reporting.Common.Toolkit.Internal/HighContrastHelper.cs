using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    public static class HighContrastHelper
    {
        internal static HighContrastTheme CurrentTheme
        {
            get
            {
                if (!HighContrastHelper.IsHighContrastOn())
                    return HighContrastTheme.None;
                return !HighContrastHelper.IsHighContrastWhiteOn() ? HighContrastTheme.Black : HighContrastTheme.White;
            }
        }

        public static bool IsHighContrastOn()
        {
            if (Environment.OSVersion.Platform == PlatformID.MacOSX)
                return false;
            return SystemParameters.HighContrast;
        }

        public static bool IsHighContrastBlack()
        {
            if (HighContrastHelper.IsHighContrastOn())
                return !HighContrastHelper.IsHighContrastWhiteOn();
            return false;
        }

        internal static bool IsHighContrastWhiteOn()
        {
            if (SystemColors.DesktopColor == Colors.White && SystemColors.ControlColor == Colors.White && (SystemColors.WindowColor == Colors.White && SystemColors.ControlTextColor == Colors.Black) && SystemColors.HighlightColor == Colors.Black)
                return SystemColors.WindowFrameColor == Colors.Black;
            return false;
        }

        internal static HighContrastTheme GetTheme(object value)
        {
            if (value is HighContrastTheme)
                return (HighContrastTheme)value;
            return HighContrastHelper.CurrentTheme;
        }
    }
}
