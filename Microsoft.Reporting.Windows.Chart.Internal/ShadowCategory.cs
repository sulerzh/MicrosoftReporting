using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class ShadowCategory : Category
    {
        private IList _childrenItemsList;

        protected override void OnChildrenSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            this._childrenItemsList = null;
        }

        internal override IList GetChildrenItemsList()
        {
            if (this._childrenItemsList == null)
            {
                IEnumerable childrenSource = this.ChildrenSource;
                if (childrenSource == null)
                    return null;
                this._childrenItemsList = childrenSource as IList;
                if (this._childrenItemsList == null)
                {
                    this._childrenItemsList = new List<object>();
                    foreach (object obj in childrenSource)
                        this._childrenItemsList.Add(obj);
                }
            }
            return this._childrenItemsList;
        }
    }
}
