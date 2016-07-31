using Microsoft.Reporting.Windows.Common.Internal.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class UpdateSession
    {
        private Collection<IUpdatable> _elementsToUpdate = new Collection<IUpdatable>();
        private Dictionary<object, Action> _actionsToExecuteBeforeUpdating = new Dictionary<object, Action>();
        private Dictionary<object, Action> _actionsExecutedWhileUpdating = new Dictionary<object, Action>();
        private Dictionary<object, Action> _actionsToExecuteAfterUpdating = new Dictionary<object, Action>();
        private Dictionary<object, Action> _actionsToExecuteOnUIThread = new Dictionary<object, Action>();
        private Dictionary<string, UpdateSession.CounterInfo> _counters = new Dictionary<string, UpdateSession.CounterInfo>();
        private SynchronizationContext uiThreadSynchContext;
        private DateTime _start;
        private bool _disableUpdates;
        private volatile int _updateNestLevel;
        private bool _isUpdatingStarted;
        private bool _isExecutingAfterUpdatingStarted;
        private bool _isBeginInvokeStarted;

        public bool DisableUpdates
        {
            get
            {
                return this._disableUpdates;
            }
            set
            {
                if (this._disableUpdates == value)
                    return;
                if (this._updateNestLevel > 0)
                    throw new ArgumentException(Resources.UpdateSession_DisableUpdates_Invalid_Call_Point);
                this._disableUpdates = value;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return DateTime.Now.Subtract(this._start);
            }
        }

        public bool IsUpdating
        {
            get
            {
                return this._updateNestLevel > 0;
            }
        }

        public bool IsExecutingAfterUpdatingStarted
        {
            get
            {
                return this._isExecutingAfterUpdatingStarted;
            }
        }

        public UpdateSession(SynchronizationContext uiThreadSynchContext)
        {
            this.uiThreadSynchContext = uiThreadSynchContext;
            this.DisableUpdates = false;
        }

        public virtual void BeginUpdates()
        {
            if (this._updateNestLevel == 0)
                this._start = DateTime.Now;
            ++this._updateNestLevel;
        }

        public virtual void Update(IUpdatable element)
        {
            if (this.DisableUpdates)
                return;
            if (this._updateNestLevel == 0)
            {
                element.Update();
            }
            else
            {
                if (this._elementsToUpdate.Contains(element) || element.Parent != null && this._elementsToUpdate.Contains(element.Parent))
                    return;
                this._elementsToUpdate.Add(element);
            }
        }

        public virtual void SkipUpdate(IUpdatable element)
        {
            if (!this._elementsToUpdate.Contains(element))
                return;
            this._elementsToUpdate.Remove(element);
        }

        public virtual void EndUpdates()
        {
            if (this._updateNestLevel == 1)
            {
                int count1 = this._elementsToUpdate.Count;
                int count2 = this._actionsToExecuteBeforeUpdating.Count;
                KeyValuePair<object, Action> keyValuePair;
                do
                {
                    keyValuePair = this._actionsToExecuteBeforeUpdating.FirstOrDefault<KeyValuePair<object, Action>>();
                    if (keyValuePair.Value != null)
                    {
                        keyValuePair.Value();
                        this._actionsToExecuteBeforeUpdating.Remove(keyValuePair.Key);
                    }
                }
                while (keyValuePair.Value != null);
                this._isUpdatingStarted = true;
                while (this._elementsToUpdate.Count > 0)
                {
                    IUpdatable updatable = this._elementsToUpdate[0];
                    if (updatable.Parent == null || !this._elementsToUpdate.Contains(updatable.Parent))
                        updatable.Update();
                    this._elementsToUpdate.RemoveAt(0);
                }
                this._actionsExecutedWhileUpdating.Clear();
                this._isExecutingAfterUpdatingStarted = true;
                this._actionsToExecuteAfterUpdating.ForEachWithIndex<KeyValuePair<object, Action>>((item, index) => item.Value());
                this._actionsToExecuteAfterUpdating.Clear();
                this._isUpdatingStarted = false;
                this._isExecutingAfterUpdatingStarted = false;
                foreach (KeyValuePair<string, UpdateSession.CounterInfo> counter in this._counters)
                {
                    int num = counter.Value.Duration != TimeSpan.Zero ? 1 : 0;
                }
                this._counters.Clear();
            }
            --this._updateNestLevel;
        }

        public void ExecuteIfPending(object actionKey)
        {
            if (this._actionsToExecuteBeforeUpdating.ContainsKey(actionKey))
            {
                this._actionsToExecuteBeforeUpdating[actionKey]();
                this._actionsToExecuteBeforeUpdating.Remove(actionKey);
            }
            if (this._actionsExecutedWhileUpdating.ContainsKey(actionKey))
            {
                this._actionsExecutedWhileUpdating[actionKey]();
                this._actionsExecutedWhileUpdating.Remove(actionKey);
            }
            if (!this._actionsToExecuteAfterUpdating.ContainsKey(actionKey))
                return;
            this._actionsToExecuteAfterUpdating[actionKey]();
            this._actionsToExecuteAfterUpdating.Remove(actionKey);
        }

        public void ExecuteOnceBeforeUpdating(Action action, object actionKey)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (actionKey == null)
                throw new ArgumentNullException("actionKey");
            if (this.DisableUpdates)
                return;
            if (this._updateNestLevel == 0 || this._isUpdatingStarted)
            {
                action();
            }
            else
            {
                if (this._actionsToExecuteBeforeUpdating.ContainsKey(actionKey))
                    return;
                this._actionsToExecuteBeforeUpdating.Add(actionKey, action);
            }
        }

        public void ExecuteOnceAfterUpdating(Action action, object actionKey, string actionName = null)
        {
            if (actionName != null)
                actionKey = new Tuple<object, string>(actionKey, actionName);
            if (action == null)
                throw new ArgumentNullException("action");
            if (actionKey == null)
                throw new ArgumentNullException("actionKey");
            if (this.DisableUpdates)
                return;
            if (this._updateNestLevel == 0 || this._isExecutingAfterUpdatingStarted)
            {
                action();
            }
            else
            {
                if (this._actionsToExecuteAfterUpdating.ContainsKey(actionKey))
                    return;
                this._actionsToExecuteAfterUpdating.Add(actionKey, action);
            }
        }

        public void ExecuteOnceDuringUpdating(Action action, object actionKey)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (actionKey == null)
                throw new ArgumentNullException("actionKey");
            if (this.DisableUpdates)
                return;
            if (this._updateNestLevel == 0)
            {
                action();
            }
            else
            {
                if (this._actionsExecutedWhileUpdating.ContainsKey(actionKey))
                    return;
                action();
                this._actionsExecutedWhileUpdating.Add(actionKey, action);
            }
        }

        public void PostExecuteOnceOnUIThread(Action action, object actionKey)
        {
            if (this._actionsToExecuteOnUIThread.ContainsKey(actionKey))
                return;
            this._actionsToExecuteOnUIThread.Add(actionKey, action);
            if (this._isBeginInvokeStarted)
                return;
            this._isBeginInvokeStarted = true;
            this.uiThreadSynchContext.Post(_ =>
           {
               while (this._actionsToExecuteOnUIThread.Count > 0)
               {
                   object key = this._actionsToExecuteOnUIThread.Keys.FirstOrDefault<object>();
                   this._actionsToExecuteOnUIThread[key]();
                   this._actionsToExecuteOnUIThread.Remove(key);
               }
               this._isBeginInvokeStarted = false;
           }, null);
        }

        public void AddCounter(string name)
        {
            this.AddCounter(name, TimeSpan.Zero);
        }

        public void AddCounter(string name, TimeSpan duration)
        {
            UpdateSession.CounterInfo counterInfo;
            if (this._counters.TryGetValue(name, out counterInfo))
            {
                ++counterInfo.Counter;
                counterInfo.Duration += duration;
                this._counters[name] = counterInfo;
            }
            else
            {
                counterInfo.Counter = 1;
                this._counters.Add(name, counterInfo);
            }
        }

        internal int GetCounter(string name)
        {
            UpdateSession.CounterInfo counterInfo;
            if (this._counters.TryGetValue(name, out counterInfo))
                return counterInfo.Counter;
            return 0;
        }

        internal void ResetCounter(string name)
        {
            if (!this._counters.ContainsKey(name))
                return;
            this._counters.Remove(name);
        }

        private struct CounterInfo
        {
            internal int Counter;
            internal TimeSpan Duration;
        }
    }
}
