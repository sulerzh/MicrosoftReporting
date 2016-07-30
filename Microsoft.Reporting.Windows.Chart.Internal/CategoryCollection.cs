using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class CategoryCollection : ObservableCollection<Category>
    {
        public event EventHandler CollectionResetting;

        public CategoryCollection()
        {
        }

        public CategoryCollection(IEnumerable<Category> categories)
        {
            foreach (Category category in categories)
                this.Add(category);
        }

        public Category Add(object key, string text)
        {
            Category category = new Category() { Key = key, Content = text };
            this.Add(category);
            return category;
        }

        protected override void ClearItems()
        {
            this.OnCollectionResetting();
            base.ClearItems();
        }

        protected virtual void OnCollectionResetting()
        {
            if (this.CollectionResetting == null)
                return;
            this.CollectionResetting(this, EventArgs.Empty);
        }
    }
}
