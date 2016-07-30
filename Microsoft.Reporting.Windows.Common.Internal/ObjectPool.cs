using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class ObjectPool<T, TContext>
    {
        private Func<T> _createObject;
        private Action<T, TContext> _initializeObject;
        private Action<T> _resetObject;
        private List<T> _objects;
        private int _currentIndex;

        public int MinimumObjectsInThePool { get; set; }

        public int MaximumObjectsInThePool { get; set; }

        public ObjectPool(Func<T> createObject, Action<T, TContext> initializeObject, Action<T> resetObject)
        {
            this.MinimumObjectsInThePool = 20;
            this.MaximumObjectsInThePool = 500;
            this._objects = new List<T>(this.MinimumObjectsInThePool);
            this._createObject = createObject;
            this._initializeObject = initializeObject;
            this._resetObject = resetObject;
        }

        public ObjectPool(Func<T> createObject)
          : this(createObject, null, null)
        {
        }

        public T Get(TContext owner)
        {
            if (this._currentIndex == this._objects.Count)
                this._objects.Add(this._createObject());
            T obj = this._objects[this._currentIndex];
            ++this._currentIndex;
            if (this._initializeObject != null)
                this._initializeObject(obj, owner);
            return obj;
        }

        public void ReleaseAll()
        {
            if (this._resetObject != null)
                this._objects.ForEach(item => this._resetObject(item));
            this._currentIndex = 0;
        }

        public void Release(T obj)
        {
            if (obj == null || !this._objects.Contains(obj))
                return;
            if (this._resetObject != null)
                this._resetObject(obj);
            this._objects.Remove(obj);
            if (this._objects.Count - this._currentIndex < this.MaximumObjectsInThePool)
                this._objects.Add(obj);
            --this._currentIndex;
        }

        public bool Contains(T obj)
        {
            return this._objects.Contains(obj);
        }

        public void AdjustPoolSize()
        {
            if (this._currentIndex + this.MinimumObjectsInThePool >= this._objects.Count)
                return;
            while (this._objects.Count > this._currentIndex + this.MinimumObjectsInThePool)
            {
                if (this._resetObject != null)
                    this._resetObject(this._objects[this._objects.Count - 1]);
                this._objects.RemoveAt(this._objects.Count - 1);
            }
        }

        public void Clear()
        {
            this.ReleaseAll();
            this._objects.Clear();
        }
    }
}
