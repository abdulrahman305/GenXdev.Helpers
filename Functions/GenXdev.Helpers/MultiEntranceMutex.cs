// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : MultiEntranceMutex.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 2.1.2025
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



using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace GenXdev.Helpers
{
    /// <summary>
    /// <para type="synopsis">
    /// Provides a multi-entrance mutex implementation that allows multiple threads
    /// within the same process to acquire the same mutex without blocking.
    /// </para>
    ///
    /// <para type="description">
    /// The MultiEntranceMutex class implements a mutex that supports re-entrance
    /// within the same process. It uses a combination of a system-wide Mutex and
    /// a local lock to manage concurrent access. The mutex is identified by a GUID
    /// and can be used to synchronize access to shared resources across different
    /// instances of the application.
    ///
    /// Key features:
    /// - Supports multiple entrances from the same process
    /// - Uses a global system mutex for cross-process synchronization
    /// - Provides timeout support for lock acquisition
    /// - Implements IDisposable for proper resource cleanup
    /// - Thread-safe operations using Interlocked and Monitor
    /// </para>
    /// </summary>
    public class MultiEntranceMutex : IDisposable
    {
        /// <summary>
        /// Gets the unique identifier for this mutex instance.
        /// </summary>
        internal Guid Id { get; private set; }

        /// <summary>
        /// The underlying system mutex used for cross-process synchronization.
        /// </summary>
        internal Mutex Mutex;

        /// <summary>
        /// Object used for locking to ensure thread safety within the process.
        /// </summary>
        internal object padLock = new object();

        /// <summary>
        /// Tracks the number of times the mutex has been acquired within this process.
        /// Used to implement re-entrance logic.
        /// </summary>
        internal long State;

        /// <summary>
        /// Attempts to acquire the mutex with an optional timeout.
        /// </summary>
        /// <param name="timeout">
        /// The maximum time to wait for the mutex. If null, waits indefinitely.
        /// </param>
        /// <returns>
        /// True if the mutex was acquired successfully, false if timeout occurred.
        /// </returns>
        public bool Lock(TimeSpan? timeout = null)
        {
            var HasHandle = false;

            try
            {
                if (timeout.HasValue)
                {
                    // Enter the local lock to ensure thread safety
                    Monitor.Enter(padLock);

                    // Increment the state counter atomically
                    if (Interlocked.Increment(ref State) == 1)
                    {
                        // First entrance: acquire the system mutex with timeout
                        HasHandle = Mutex.WaitOne(
                            Convert.ToInt32(Math.Round(timeout.Value.TotalMilliseconds, 0)),
                            false);

                        if (HasHandle == false)
                        {
                            // Timeout occurred, decrement state and exit lock
                            Interlocked.Decrement(ref State);

                            Monitor.Exit(padLock);

                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    // Enter the local lock to ensure thread safety
                    Monitor.Enter(padLock);

                    // Increment the state counter atomically
                    if (Interlocked.Increment(ref State) == 1)
                    {
                        // First entrance: acquire the system mutex indefinitely
                        Mutex.WaitOne(Timeout.Infinite, false);
                    }

                    return true;
                }
            }
            catch (AbandonedMutexException)
            {
                // Mutex was abandoned by another process, but we can still use it
                HasHandle = true;

                return true;
            }
        }

        /// <summary>
        /// Releases the mutex if this is the last entrance.
        /// </summary>
        public void Unlock()
        {
            // Decrement the state counter atomically
            if (Interlocked.Decrement(ref State) == 0)
            {
                // Last entrance: release the system mutex
                Mutex.ReleaseMutex();
            }

            // Exit the local lock
            Monitor.Exit(padLock);
        }

        /// <summary>
        /// Gets a value indicating whether the mutex is currently locked by another process.
        /// </summary>
        /// <returns>
        /// True if the mutex is locked elsewhere, false if available.
        /// </returns>
        public bool IsLockedElsewhere
        {
            get
            {
                // Try to acquire the lock with a short timeout
                if (Lock(TimeSpan.FromMilliseconds(500)))
                {
                    // Successfully acquired, so release it immediately
                    Unlock();

                    return false;
                }

                // Could not acquire within timeout, so it's locked elsewhere
                return true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the MultiEntranceMutex class.
        /// </summary>
        /// <param name="Id">
        /// The unique identifier for the mutex. If null, uses the assembly's GUID.
        /// </param>
        public MultiEntranceMutex(Guid? Id)
        {
            if (Id.HasValue)
            {
                this.Id = Id.Value;
            }
            else
            {
                // Extract GUID from the assembly's GuidAttribute
                this.Id = Guid.Parse(
                    ((GuidAttribute)Assembly.GetExecutingAssembly()
                        .GetCustomAttributes(typeof(GuidAttribute), false)
                        .GetValue(0)).Value);
            }

            // Create a global system mutex with the GUID as identifier
            this.Mutex = new Mutex(false,
                string.Format("Global\\{{{0}}}", this.Id),
                out bool createdNew);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and optionally managed resources.
        /// </summary>
        /// <param name="disposing">
        /// True to release both managed and unmanaged resources;
        /// false to release only unmanaged resources.
        /// </param>
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

        /// <summary>
        /// Finalizer for the MultiEntranceMutex class.
        /// </summary>
        ~MultiEntranceMutex()
        {
            Dispose(false);
        }
    }
}