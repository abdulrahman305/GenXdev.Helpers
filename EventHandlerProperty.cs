namespace GenXdev.Events
{
    public class EventHandlerProperty<T> where T : EventArgs
    {
        public event EventHandler<T> OnTriggered;

        protected virtual void TriggerEvent(object sender, params object[] eventArgConstructorArguments)
        {
            try
            {
                var handler = OnTriggered;

                if (handler != null)
                {
                    var args = (T)Activator.CreateInstance(typeof(T), eventArgConstructorArguments);

                    handler(sender, args);
                }
            }
            catch { }
        }

        protected virtual void TriggerEvent(object sender, T args)
        {
            try
            {
                var handler = OnTriggered;

                if (handler != null)
                {
                    handler(sender, args);
                }
            }
            catch { }
        }

        protected virtual void TriggerEvent(bool async, object sender, params object[] eventArgConstructorArguments)
        {
            if (async)
            {
                TriggerAsyncEvent(sender, eventArgConstructorArguments);
            }
            else
            {
                TriggerEvent(sender, eventArgConstructorArguments);
            }
        }

        protected virtual void TriggerEvent(bool async, object sender, T args)
        {
            if (async)
            {
                TriggerAsyncEvent(sender, args);
            }
            else
            {
                TriggerEvent(sender, args);
            }
        }

        protected virtual void TriggerAsyncEvent(object sender, params object[] eventArgConstructorArguments)
        {

            ThreadPool.QueueUserWorkItem(
                (object state) =>
                {
                    TriggerEvent(sender, eventArgConstructorArguments);
                }
            );
        }

        protected virtual void TriggerAsyncEvent(object sender, T args)
        {

            ThreadPool.QueueUserWorkItem(
                (object state) =>
                {
                    TriggerEvent(sender, args);
                }
            );
        }
    }
}
