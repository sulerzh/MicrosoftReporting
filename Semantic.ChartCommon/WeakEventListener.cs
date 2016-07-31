namespace System.Windows.Controls
{
    public class WeakEventListener<TInstance, TSource, TEventArgs> where TInstance : class
    {
        private WeakReference _weakInstance;

        public Action<TInstance, TSource, TEventArgs> OnEventAction { get; set; }

        public Action<WeakEventListener<TInstance, TSource, TEventArgs>> OnDetachAction { get; set; }

        public WeakEventListener(TInstance instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            this._weakInstance = new WeakReference(instance);
        }

        public void OnEvent(TSource source, TEventArgs eventArgs)
        {
            TInstance instance = (TInstance)this._weakInstance.Target;
            if (instance != null)
            {
                if (this.OnEventAction == null)
                    return;
                this.OnEventAction(instance, source, eventArgs);
            }
            else
                this.Detach();
        }

        public void Detach()
        {
            if (this.OnDetachAction == null)
                return;
            this.OnDetachAction(this);
            this.OnDetachAction = null;
        }
    }
}
