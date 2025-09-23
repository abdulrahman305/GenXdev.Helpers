// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : HiPerfTimer.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.284.2025
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



namespace GenXdev.Additional.HiPerfTimer
{
    public class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
        private long startTime;
        private long stopTime;
        public bool stopped = true;
        private long freq;

        public NativeMethods(bool StartNow = false)
        {
            Interlocked.Exchange(ref startTime, 0);
            Interlocked.Exchange(ref stopTime, 0);
            freq = 0;
            if (QueryPerformanceFrequency(out freq) == false)
            {
                throw new Exception("Timer not supported");
            }
            if (StartNow)
                Start();
        }

        /// <summary>
        /// Start the timer
        /// </summary>
        /// <returns>long - tick count</returns>
        public long Start()
        {
            stopped = false;
            //startTime2 = DateTime.Now;
            QueryPerformanceCounter(out long startTimeTmp);
            Interlocked.Exchange(ref startTime, startTimeTmp);
            return startTimeTmp;
        }

        /// <summary>
        /// Stop timer 
        /// </summary>
        /// <returns>long - tick count</returns>
        public long Stop()
        {
            stopped = true;
            //stopTime2 = DateTime.Now;
            QueryPerformanceCounter(out long stopTimeTmp);
            Interlocked.Exchange(ref stopTime, stopTimeTmp);
            return stopTimeTmp;
        }

        /// <summary>
        /// Return the duration of the timer (in seconds)
        /// </summary>
        /// <returns>double - duration</returns>
        public double Duration
        {
            get
            {
                if (stopped)
                {
                    return Convert.ToDouble(Interlocked.Read(ref stopTime) - Interlocked.Read(ref startTime)) / freq;
                }
                QueryPerformanceCounter(out long now);
                return Convert.ToDouble(now - Interlocked.Read(ref startTime)) / freq;
            }
        }

        /// <summary>
        /// Frequency of timer (no counts in one second on this machine)
        /// </summary>
        ///<returns>long - Frequency</returns>
        public long Frequency
        {
            get
            {
                return freq;
            }
        }

        public void Reset()
        {
            QueryPerformanceCounter(out long now);
            Interlocked.Exchange(ref startTime, now);
        }
    }
}
