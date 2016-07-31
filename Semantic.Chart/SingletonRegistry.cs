using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class SingletonRegistry
    {
        private Dictionary<object, Singleton> _singletons = new Dictionary<object, Singleton>();

        public object GetSingleton(object key, Func<object> createSingleton, Action<object> disposeSingleton)
        {
            Singleton singleton;
            if (!this._singletons.TryGetValue(key, out singleton))
            {
                singleton = new Singleton()
                {
                    Instance = createSingleton(),
                    DisposeAction = disposeSingleton
                };
                this._singletons.Add(key, singleton);
            }
            ++singleton.ReferenceCounter;
            return singleton.Instance;
        }

        public void ReleaseSingleton(object key)
        {
            Singleton singleton;
            if (!this._singletons.TryGetValue(key, out singleton))
                return;
            --singleton.ReferenceCounter;
            if (singleton.ReferenceCounter != 0)
                return;
            if (singleton.DisposeAction != null)
                singleton.DisposeAction(singleton.Instance);
            this._singletons.Remove(key);
        }
    }
}
