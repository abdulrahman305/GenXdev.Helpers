// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : HiPerfTimer.cs
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
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace GenXdev.Helpers
{
    /// <summary>
    /// Provides high-performance timing functionality using the Windows QueryPerformanceCounter API.
    /// This class allows precise measurement of elapsed time with microsecond accuracy.
    /// </summary>
    public class NativeMethods
    {
        // P/Invoke declarations for Windows performance counter functions
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
        // Stores the start time in performance counter ticks
        private long startTime;
        // Stores the stop time in performance counter ticks
        private long stopTime;
        // Indicates whether the timer is currently stopped
        public bool stopped = true;
        // Stores the performance counter frequency
        private long freq;
        /// <summary>
        /// Initializes a new instance of the NativeMethods class.
        /// </summary>
        /// <param name="StartNow">If true, starts the timer immediately upon construction.</param>
        public NativeMethods(bool StartNow = false)
        {

            // Initialize start and stop times to zero
            Interlocked.Exchange(ref startTime, 0);

            Interlocked.Exchange(ref stopTime, 0);

            // Initialize frequency to zero
            freq = 0;

            // Query the performance frequency
            if (QueryPerformanceFrequency(out freq) == false)
            {

                throw new Exception("Timer not supported");
            }

            // Start the timer if requested
            if (StartNow)
                Start();
        }

        /// <summary>
        /// Starts the high-performance timer.
        /// </summary>
        /// <returns>The performance counter value when the timer was started.</returns>
        public long Start()
        {

            // Mark timer as running
            stopped = false;

            //startTime2 = DateTime.Now;
            // Get current performance counter value
            QueryPerformanceCounter(out long startTimeTmp);

            // Atomically set the start time
            Interlocked.Exchange(ref startTime, startTimeTmp);

            return startTimeTmp;
        }

        /// <summary>
        /// Stops the high-performance timer.
        /// </summary>
        /// <returns>The performance counter value when the timer was stopped.</returns>
        public long Stop()
        {

            // Mark timer as stopped
            stopped = true;

            //stopTime2 = DateTime.Now;
            // Get current performance counter value
            QueryPerformanceCounter(out long stopTimeTmp);

            // Atomically set the stop time
            Interlocked.Exchange(ref stopTime, stopTimeTmp);

            return stopTimeTmp;
        }

        /// <summary>
        /// Gets the duration elapsed since the timer was started, in seconds.
        /// </summary>
        /// <returns>The duration in seconds.</returns>
        public double Duration
        {
            get
            {

                if (stopped)
                {

                    // Calculate duration from start to stop time
                    return Convert.ToDouble(Interlocked.Read(ref stopTime) - Interlocked.Read(ref startTime)) / freq;
                }

                // Get current performance counter value
                QueryPerformanceCounter(out long now);

                // Calculate duration from start to current time
                return Convert.ToDouble(now - Interlocked.Read(ref startTime)) / freq;
            }
        }

        /// <summary>
        /// Gets the frequency of the performance counter, representing the number of ticks per second.
        /// </summary>
        /// <returns>The frequency in ticks per second.</returns>
        public long Frequency
        {
            get
            {

                return freq;
            }
        }

        /// <summary>
        /// Resets the start time to the current performance counter value.
        /// </summary>
        public void Reset()
        {

            // Get current performance counter value
            QueryPerformanceCounter(out long now);

            // Atomically set the start time to current time
            Interlocked.Exchange(ref startTime, now);
        }
    }
}
