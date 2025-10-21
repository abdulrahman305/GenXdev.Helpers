// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : MultiEntranceMutex.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.304.2025
// ################################################################################
// Copyright (c)  René Vaessen / GenXdev
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ################################################################################



using System.Reflection;
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
                        HasHandle = Mutex.WaitOne(Convert.ToInt32(System.Math.Round(timeout.Value.TotalMilliseconds, 0)), false);
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