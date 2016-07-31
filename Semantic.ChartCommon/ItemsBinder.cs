using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public abstract class ItemsBinder<T> : IItemsBinder<T>
    {
        private ItemsBinder<T>.SourceDictionary _sources = new ItemsBinder<T>.SourceDictionary();

        public virtual void Bind(T target, object source)
        {
            INotifyPropertyChanged propertyChanged = source as INotifyPropertyChanged;
            if (propertyChanged != null)
            {
                ItemsBinder<T>.TargetDictionary targetDictionary = null;
                if (!this._sources.TryGetValue(source, out targetDictionary))
                {
                    targetDictionary = new ItemsBinder<T>.TargetDictionary();
                    this._sources.Add(source, targetDictionary);
                }
                ItemsBinder<T>.TargetEventListener targetEventListener = null;
                if (!targetDictionary.TryGetValue(target, out targetEventListener))
                {
                    targetEventListener = new ItemsBinder<T>.TargetEventListener(this);
                    targetEventListener.OnEventAction = (instance, sender, eventArgs) => instance.SourcePropertyChanged(sender, eventArgs);
                    targetEventListener.OnDetachAction = weakEventListener => propertyChanged.PropertyChanged -= new PropertyChangedEventHandler(weakEventListener.OnEvent);
                    propertyChanged.PropertyChanged += new PropertyChangedEventHandler(targetEventListener.OnEvent);
                    targetDictionary.Add(target, targetEventListener);
                }
            }
            this.CallUpdateTarget(target, source, null);
        }

        public virtual void Unbind(T target, object source)
        {
            ItemsBinder<T>.TargetDictionary targetDictionary = null;
            if (!this._sources.TryGetValue(source, out targetDictionary))
                return;
            ItemsBinder<T>.TargetEventListener targetEventListener = null;
            if (targetDictionary.TryGetValue(target, out targetEventListener))
            {
                targetEventListener.Detach();
                targetDictionary.Remove(target);
            }
            if (targetDictionary.Count != 0)
                return;
            this._sources.Remove(source);
        }

        private void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemsBinder<T>.TargetDictionary targetDictionary = null;
            if (!this._sources.TryGetValue(sender, out targetDictionary))
                return;
            foreach (T key in targetDictionary.Keys)
                this.CallUpdateTarget(key, sender, e.PropertyName);
        }

        private void CallUpdateTarget(T target, object source, string propertyName)
        {
            IUpdateSessionProvider updateSessionProvider = target as IUpdateSessionProvider;
            if (updateSessionProvider != null && updateSessionProvider.UpdateSession != null)
                updateSessionProvider.UpdateSession.BeginUpdates();
            this.UpdateTarget(target, source, propertyName);
            if (updateSessionProvider == null || updateSessionProvider.UpdateSession == null)
                return;
            updateSessionProvider.UpdateSession.EndUpdates();
        }

        public abstract void UpdateTarget(T target, object source, string propertyName);

        private class TargetEventListener : WeakEventListener<ItemsBinder<T>, object, PropertyChangedEventArgs>
        {
            public TargetEventListener(ItemsBinder<T> binder)
              : base(binder)
            {
            }
        }

        private class TargetDictionary : Dictionary<T, ItemsBinder<T>.TargetEventListener>
        {
        }

        private class SourceDictionary : Dictionary<object, ItemsBinder<T>.TargetDictionary>
        {
        }
    }
}
