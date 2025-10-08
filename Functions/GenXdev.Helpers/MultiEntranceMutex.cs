// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : MultiEntranceMutex.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.298.2025
// ################################################################################
// MIT License
//
// Copyright 2021-2025 GenXdev
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
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