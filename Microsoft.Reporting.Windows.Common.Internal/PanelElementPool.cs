using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class PanelElementPool<T, TContext> where T : UIElement
    {
        private List<T> _elements = new List<T>();
        private Panel _panel;
        private ObjectPool<T, TContext> _objectPool;
        private int _firstNotUsedElementIndex;
        private Action<T, TContext> _initializeAction;
        private Action<T> _resetAction;

        public int MaxElementCount { get; set; }

        public int ReservedTopElements { get; set; }

        public int InUseElementCount
        {
            get
            {
                return this._firstNotUsedElementIndex;
            }
        }

        public PanelElementPool(Panel panel, ObjectPool<T, TContext> objectPool)
        {
            this.MaxElementCount = 0;
            this.ReservedTopElements = 0;
            this._panel = panel;
            this._objectPool = objectPool;
        }

        public PanelElementPool(Panel panel, Func<T> createObject, Action<T, TContext> initializeObject, Action<T> resetObject)
          : this(panel, new ObjectPool<T, TContext>(createObject, initializeObject, resetObject))
        {
            this._initializeAction = initializeObject;
            this._resetAction = resetObject;
        }

        public void ReleaseAll()
        {
            if (this._resetAction != null)
            {
                foreach (T element in this._elements)
                    this._resetAction(element);
            }
            this._firstNotUsedElementIndex = 0;
        }

        public void Release(T element)
        {
            if (this._resetAction != null)
                this._resetAction(element);
            if (!this._elements.Contains(element))
                return;
            this._elements.Remove(element);
            this._elements.Add(element);
            --this._firstNotUsedElementIndex;
        }

        public T Get(TContext context)
        {
            T obj1 = default(T);
            T obj2;
            if (this._firstNotUsedElementIndex < this._elements.Count)
            {
                obj2 = this._elements[this._firstNotUsedElementIndex];
                if (obj2.Visibility == Visibility.Collapsed)
                    obj2.Visibility = Visibility.Visible;
                if (this._initializeAction != null)
                    this._initializeAction(obj2, context);
            }
            else
            {
                obj2 = this._objectPool.Get(context);
                this._elements.Add(obj2);
                int index = this._panel.Children.Count - this.ReservedTopElements;
                if (this.ReservedTopElements > 0 && index > 0)
                    this._panel.Children.Insert(index, obj2);
                else
                    this._panel.Children.Add(obj2);
            }
            ++this._firstNotUsedElementIndex;
            return obj2;
        }

        public void AdjustPoolSize()
        {
            while (this._elements.Count - this._firstNotUsedElementIndex > Math.Max(0, this.MaxElementCount))
            {
                T obj = this._elements[this._firstNotUsedElementIndex];
                this._elements.RemoveAt(this._firstNotUsedElementIndex);
                this._panel.Children.Remove(obj);
                this._objectPool.Release(obj);
            }
            for (int index = this._firstNotUsedElementIndex; index < this._elements.Count; ++index)
                this._elements[index].Visibility = Visibility.Collapsed;
            this._objectPool.AdjustPoolSize();
        }

        public void Clear()
        {
            this.ReleaseAll();
            foreach (T element in this._elements)
                this._panel.Children.Remove(element);
            this._elements.Clear();
            this._objectPool.Clear();
        }
    }
}
