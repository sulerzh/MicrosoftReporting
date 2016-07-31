using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class StoryboardInfo
    {
        public Storyboard Storyboard { get; set; }

        public Action ReleaseAction { get; set; }

        public DependencyObject StoryboardTarget { get; set; }

        public object AnimateFrom { get; set; }
    }
}
