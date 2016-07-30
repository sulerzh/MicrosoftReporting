using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class Category : FrameworkElement, INotifyPropertyChanged, INotifyTreeChanged
    {
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(object), typeof(Category), new PropertyMetadata(null, new PropertyChangedCallback(Category.OnKeyChanged)));
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(Category), new PropertyMetadata(null, new PropertyChangedCallback(Category.OnContentChanged)));
        public static readonly DependencyProperty ChildrenSourceProperty = DependencyProperty.Register("ChildrenSource", typeof(IEnumerable), typeof(Category), new PropertyMetadata(null, new PropertyChangedCallback(Category.OnChildrenSourceChanged)));
        private bool _isLeaf = true;
        private const string KeyPropertyName = "Key";
        private const string ContentPropertyName = "Content";
        private const string ChildrenSourcePropertyName = "ChildrenSource";
        private ObservableCollectionListSynchronizer<Category> _childrenAdapter;
        private CategoryCollection _children;

        internal Range<int> LeafIndexRange { get; set; }

        internal CategoryScale Scale { get; set; }

        internal Category ParentCategory { get; set; }

        public object Key
        {
            get
            {
                return this.GetValue(Category.KeyProperty);
            }
            set
            {
                this.SetValue(Category.KeyProperty, value);
            }
        }

        public object Content
        {
            get
            {
                return this.GetValue(Category.ContentProperty);
            }
            set
            {
                this.SetValue(Category.ContentProperty, value);
            }
        }

        public IEnumerable ChildrenSource
        {
            get
            {
                return (IEnumerable)this.GetValue(Category.ChildrenSourceProperty);
            }
            set
            {
                this.SetValue(Category.ChildrenSourceProperty, value);
            }
        }

        public CategoryCollection Children
        {
            get
            {
                return this._children;
            }
            set
            {
                if (this._children == value)
                    return;
                this.OnRemoveChildren(this._children);
                this._children = value;
                this.OnAddChildren(this._children);
                this.TreeChanged(this, new TreeChangedEventArgs(this, EventArgs.Empty));
            }
        }

        public bool IsLeaf
        {
            get
            {
                return this._isLeaf;
            }
            private set
            {
                if (this._isLeaf == value)
                    return;
                this._isLeaf = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("IsLeaf"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<TreeChangedEventArgs> TreeChanged;

        public Category()
        {
            this._children = new CategoryCollection();
            this._children.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Subcategories_CollectionChanged);
            this._children.CollectionResetting += new EventHandler(this.Children_CollectionResetting);
        }

        public Category(object key, string text, params Category[] subcategories)
        {
            this.Key = key;
            this.Content = text;
            this._children = new CategoryCollection(subcategories);
            this._children.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Subcategories_CollectionChanged);
            this._children.CollectionResetting += new EventHandler(this.Children_CollectionResetting);
        }

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Category)d).OnKeyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnKeyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs("Key"));
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Category)d).OnContentChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnContentChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs("Content"));
        }

        private static void OnChildrenSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Category)d).OnChildrenSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        protected virtual void OnChildrenSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (this.Scale == null)
            {
                this.Children.Clear();
                if (newValue == null)
                    return;
                foreach (object businessObject in newValue)
                {
                    Category category = new Category() { Scale = this.Scale };
                    this.Scale.Binder.Bind(category, businessObject);
                    this.Children.Add(category);
                }
            }
            else
            {
                if (this._childrenAdapter == null)
                {
                    this._childrenAdapter = new ObservableCollectionListSynchronizer<Category>();
                    this._childrenAdapter.TargetList = this._children;
                    this._childrenAdapter.CreateItem = item =>
                   {
                       Category category = new Category() { Scale = this.Scale };
                       this.Scale.Binder.Bind(category, item);
                       return category;
                   };
                    this._childrenAdapter.StartUpdating = () =>
                   {
                       if (this.Scale == null)
                           return;
                       this.Scale.BeginInit();
                   };
                    this._childrenAdapter.EndUpdating = () =>
                   {
                       if (this.Scale == null)
                           return;
                       this.Scale.EndInit();
                   };
                    this._childrenAdapter.ResetItems = () => this.Scale.InvalidateTree();
                }
                this._childrenAdapter.SourceCollection = newValue;
            }
        }

        public override string ToString()
        {
            return this.Content.ToString();
        }

        private void Children_CollectionResetting(object sender, EventArgs e)
        {
            this.OnRemoveChildren(this._children);
        }

        private void Subcategories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnRemoveChildren(e.OldItems);
            this.OnAddChildren(e.NewItems);
            this.IsLeaf = e.NewItems == null || e.NewItems.Count == 0;
            this.OnTreeChanged(new TreeChangedEventArgs(this, e));
        }

        private void OnAddChildren(IEnumerable categories)
        {
            if (categories == null)
                return;
            foreach (Category category in categories)
            {
                category.Scale = this.Scale;
                category.ParentCategory = this;
                category.TreeChanged += new EventHandler<TreeChangedEventArgs>(this.Child_TreeChanged);
            }
        }

        private void OnRemoveChildren(IEnumerable categories)
        {
            if (categories == null)
                return;
            foreach (Category category in categories)
            {
                category.Scale = null;
                category.ParentCategory = null;
                category.TreeChanged -= new EventHandler<TreeChangedEventArgs>(this.Child_TreeChanged);
            }
        }

        private void Child_TreeChanged(object sender, TreeChangedEventArgs e)
        {
            this.OnTreeChanged(e);
        }

        internal virtual IList GetChildrenItemsList()
        {
            return this._children;
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged == null)
                return;
            this.PropertyChanged(this, e);
        }

        protected virtual void OnTreeChanged(TreeChangedEventArgs e)
        {
            if (this.TreeChanged == null)
                return;
            this.TreeChanged(this, e);
        }
    }
}
