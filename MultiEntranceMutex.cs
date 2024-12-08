using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace GenXdev.Helpers
{
    public class MultiEntranceMutex : IDisposable
    {
        internal Guid Id { get; private set; }
        internal Mutex Mutex;
        internal object padLock = new object();
        internal long State;

        public bool Lock(TimeSpan? timeout = null)
        {
            var HasHandle = false;
            try
            {
                if (timeout.HasValue)
                {
                    Monitor.Enter(padLock);

                    if (Interlocked.Increment(ref State) == 1)
                    {
                        HasHandle = Mutex.WaitOne(Convert.ToInt32(Math.Round(timeout.Value.TotalMilliseconds, 0)), false);
                        if (HasHandle == false)
                        {
                            Interlocked.Decrement(ref State);
                            Monitor.Exit(padLock);
                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    Monitor.Enter(padLock);

                    if (Interlocked.Increment(ref State) == 1)
                    {
                        Mutex.WaitOne(Timeout.Infinite, false);
                    }

                    return true;
                }
            }
            catch (AbandonedMutexException)
            {
                HasHandle = true;
                return true;
            }
        }

        public void Unlock()
        {
            if (Interlocked.Decrement(ref State) == 0)
            {
                Mutex.ReleaseMutex();
            }

            Monitor.Exit(padLock);
        }

        public bool IsLockedElsewhere
        {
            get
            {
                if (Lock(TimeSpan.FromMilliseconds(500)))
                {
                    Unlock();

                    return false;
                }

                return true;
            }
        }

        public MultiEntranceMutex(Guid? Id)
        {
            if (Id.HasValue)
            {
                this.Id = Id.Value;
            }
            else
            {
                this.Id = Guid.Parse(((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value);
            }

            var allowEveryoneRule =
                new MutexAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    MutexRights.FullControl, AccessControlType.Allow);

            this.Mutex = new Mutex(false, string.Format("Global\\{{{0}}}", this.Id), out bool createdNew);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Mutex != null)
                {
                    Mutex.Dispose();
                    Mutex = null;
                }
            }
        }

        ~MultiEntranceMutex()
        {
            Dispose(false);
        }
    }
}