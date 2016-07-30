using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class PaletteDispenser
    {
        private List<int> _dispensedResourceDictionaryIndexes = new List<int>();
        private Dictionary<object, ResourceDictionary> _resourceDictionariesDispensed = new Dictionary<object, ResourceDictionary>();
        private IList<ResourceDictionary> _resourceDictionaries;
        private PaletteDispenser _parent;

        public IList<ResourceDictionary> ResourceDictionaries
        {
            get
            {
                return this._resourceDictionaries;
            }
            set
            {
                if (value == this._resourceDictionaries)
                    return;
                INotifyCollectionChanged collectionChanged1 = this._resourceDictionaries as INotifyCollectionChanged;
                if (collectionChanged1 != null)
                    collectionChanged1.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.ResourceDictionariesCollectionChanged);
                this._resourceDictionaries = value;
                INotifyCollectionChanged collectionChanged2 = this._resourceDictionaries as INotifyCollectionChanged;
                if (collectionChanged2 != null)
                    collectionChanged2.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ResourceDictionariesCollectionChanged);
                this.Reset();
            }
        }

        public PaletteDispenser Parent
        {
            get
            {
                return this._parent;
            }
            set
            {
                if (this._parent == value)
                    return;
                if (this._parent != null)
                    this._parent.PaletteChanged -= new EventHandler(this.ParentPaletteChanged);
                this._parent = value;
                if (this._parent != null)
                    this._parent.PaletteChanged += new EventHandler(this.ParentPaletteChanged);
                this.OnParentChanged();
            }
        }

        private bool ShoudUseParentPalette
        {
            get
            {
                if (this.Parent != null)
                    return this.ResourceDictionaries == null;
                return false;
            }
        }

        public event EventHandler PaletteChanged;

        public ResourceDictionary Next(Func<ResourceDictionary, bool> predicate, object key)
        {
            return this.Next(predicate, key, this._dispensedResourceDictionaryIndexes);
        }

        internal ResourceDictionary Next(Func<ResourceDictionary, bool> predicate, object key, List<int> dispensedIndexes)
        {
            ResourceDictionary resourceDictionary = null;
            if (key != null && this._resourceDictionariesDispensed.TryGetValue(key, out resourceDictionary))
                return resourceDictionary;
            if (this.ShoudUseParentPalette)
            {
                resourceDictionary = this.Parent.Next(predicate, key, dispensedIndexes);
            }
            else
            {
                resourceDictionary = this.Next(predicate, dispensedIndexes);
                if (resourceDictionary != null && key != null)
                    this._resourceDictionariesDispensed.Add(key, resourceDictionary);
            }
            return resourceDictionary;
        }

        private ResourceDictionary Next(Func<ResourceDictionary, bool> predicate, List<int> dispensedIndexes)
        {
            if (this.ResourceDictionaries != null && this.ResourceDictionaries.Count > 0)
            {
                List<int> intList = new List<int>();
                int index = 0;
                while (true)
                {
                    do
                    {
                        ResourceDictionary resourceDictionary = this.ResourceDictionaries[index];
                        if (predicate(resourceDictionary))
                        {
                            intList.Add(index);
                            if (!dispensedIndexes.Contains(index))
                            {
                                dispensedIndexes.Add(index);
                                return resourceDictionary;
                            }
                        }
                        index = (index + 1) % this.ResourceDictionaries.Count;
                    }
                    while (index != 0);
                    if (intList.Count != 0)
                        intList.ForEach(item => dispensedIndexes.Remove(item));
                    else
                        break;
                }
            }
            return null;
        }

        public void Reset()
        {
            this._dispensedResourceDictionaryIndexes.Clear();
            this._resourceDictionariesDispensed.Clear();
            EventHandler eventHandler = this.PaletteChanged;
            if (eventHandler == null)
                return;
            eventHandler(this, EventArgs.Empty);
        }

        private void ResourceDictionariesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && this.ResourceDictionaries.Count - e.NewItems.Count == e.NewStartingIndex)
                return;
            this.Reset();
        }

        private void OnParentChanged()
        {
            this.Reset();
        }

        private void ParentPaletteChanged(object sender, EventArgs e)
        {
            this.Reset();
        }
    }
}
