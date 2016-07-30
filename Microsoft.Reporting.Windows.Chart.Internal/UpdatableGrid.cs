using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Linq;
using System.Windows.Controls;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class UpdatableGrid : Grid, IUpdatable
    {
        public IUpdatable Parent
        {
            get
            {
                return null;
            }
        }

        public void Update()
        {
            this.Children.OfType<IUpdatable>().ForEachWithIndex<IUpdatable>((item, index) => item.Update());
        }
    }
}
