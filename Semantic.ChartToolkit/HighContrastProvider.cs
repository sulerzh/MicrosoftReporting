using System.Windows;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    public class HighContrastProvider : DependencyObject
    {
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register("Theme", typeof(HighContrastTheme), typeof(HighContrastProvider), new PropertyMetadata(HighContrastTheme.None, null));

        public HighContrastTheme Theme
        {
            get
            {
                return (HighContrastTheme)this.GetValue(HighContrastProvider.ThemeProperty);
            }
            private set
            {
                this.SetValue(HighContrastProvider.ThemeProperty, value);
            }
        }

        public HighContrastProvider()
        {
            this.Theme = HighContrastHelper.CurrentTheme;
        }
    }
}
