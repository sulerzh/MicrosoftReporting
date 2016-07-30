using Microsoft.Reporting.Windows.Chart.Internal.Properties;
using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public sealed class SeriesAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider
    {
        private Series Series
        {
            get
            {
                return (Series)this.Owner;
            }
        }

        bool ISelectionProvider.CanSelectMultiple
        {
            get
            {
                return true;
            }
        }

        bool ISelectionProvider.IsSelectionRequired
        {
            get
            {
                return false;
            }
        }

        public SeriesAutomationPeer(Series owner)
          : base(owner)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        protected override string GetClassNameCore()
        {
            return Resources.Automation_SeriesClassName;
        }

        protected override string GetAutomationIdCore()
        {
            string str = base.GetAutomationIdCore();
            if (string.IsNullOrEmpty(str))
            {
                str = this.GetName();
                if (this.Series.ChartArea != null)
                {
                    int num = this.Series.ChartArea.GetSeries().IndexOf(this.Series);
                    if (num != -1)
                        str = "Series" + num.ToString(CultureInfo.InvariantCulture);
                }
            }
            return str;
        }

        protected override string GetNameCore()
        {
            string str = base.GetNameCore();
            if (string.IsNullOrEmpty(str))
                str = string.Format(CultureInfo.CurrentCulture, Resources.SeriesScreenReaderLabel, new object[1]
                {
           this.Series.LegendText
                });
            if (string.IsNullOrEmpty(str))
                str = this.GetClassName();
            if (string.IsNullOrEmpty(str))
                str = this.Series.Name;
            if (string.IsNullOrEmpty(str))
                str = this.Series.GetType().Name;
            return str;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection)
                return this;
            return null;
        }

        protected override Rect GetBoundingRectangleCore()
        {
            if (this.Series.ChartArea == null || !this.Series.ChartArea.IsTemplateApplied)
                return base.GetBoundingRectangleCore();
            return new FrameworkElementAutomationPeer(this.Series.SeriesPresenter.RootPanel).GetBoundingRectangle();
        }

        protected override List<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> automationPeerList = new List<AutomationPeer>();
            IEnumerable<AutomationPeer> collection = this.Series.DataPoints.Where<DataPoint>(dataPoint => dataPoint.ViewState == DataPointViewState.Normal).Select<DataPoint, AutomationPeer>(dataPoint => UIElementAutomationPeer.CreatePeerForElement((UIElement)dataPoint));
            automationPeerList.AddRange(collection);
            return automationPeerList;
        }

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            return this.Series.GetSelectedDataPoints().Select<DataPoint, IRawElementProviderSimple>(dataPoint => this.ProviderFromPeer(UIElementAutomationPeer.CreatePeerForElement((UIElement)dataPoint))).ToArray<IRawElementProviderSimple>();
        }
    }
}
