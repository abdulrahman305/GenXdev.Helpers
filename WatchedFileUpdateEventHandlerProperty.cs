using GenXdev.Helpers;
using System.Collections.Concurrent;

namespace GenXdev.Events
{
    public class WatchedFileUpdateEventHandlerProperty : FileUpdateEventHandlerProperty, IDisposable
    {
        FileSystemWatcher Watcher;
        ConcurrentDictionary<string, System.Threading.Timer> WatchedFiles = new ConcurrentDictionary<string, System.Threading.Timer>();

        public WatchedFileUpdateEventHandlerProperty(
            string directory,
            string searchMask,
            bool includeSubDirectories,
            bool startNow
        )
            : base(directory, searchMask, includeSubDirectories)
        {
            if (startNow)
            {
                Start();
            }
        }

        public bool Started { get; private set; }

        public void Stop()
        {
            if (!Started)
                return;

            Started = false;

            if (Watcher != null)
            {
                Watcher.EnableRaisingEvents = false;
                Watcher.Dispose();
                Watcher = null;
            }
        }

        public void Start()
        {
            if (Started)
                return;

            Started = true;
            RecreateWatcher();
        }

        void RecreateWatcher()
        {
            if (!Started)
                return;

            try
            {
                if (Watcher != null)
                {
                    Watcher.Dispose();
                }
            }
            catch { }

            try
            {
                GenXdev.Helpers.FileSystem.ForciblyPrepareTargetDirectory(Directory);

                Watcher = new FileSystemWatcher(Directory, SearchMask);
                Watcher.IncludeSubdirectories = IncludeSubdirectories;
                Watcher.InternalBufferSize = 1024 * 64;
                Watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                Watcher.Error += new ErrorEventHandler(OnError);
                Watcher.Changed += new FileSystemEventHandler(OnChanged);
                Watcher.Created += new FileSystemEventHandler(OnChanged);
                Watcher.Deleted += new FileSystemEventHandler(OnChanged);
                Watcher.Renamed += new RenamedEventHandler(OnRenamed);

                if (Started)
                {
                    Watcher.EnableRaisingEvents = true;
                }
            }
            catch { }
        }

        void OnError(object sender, ErrorEventArgs e)
        {
            ThreadPool.QueueUserWorkItem((object state) =>
            {
                System.Threading.Thread.Sleep(10);
                RecreateWatcher();
            });
        }
        void OnRenamed(object source, RenamedEventArgs e)
        {
            OnChanged(e.FullPath);
        }

        void OnChanged(object source, FileSystemEventArgs e)
        {
            OnChanged(e.FullPath);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2002:DoNotLockOnObjectsWithWeakIdentity")]
        private void OnChanged(string fullPath)
        {
            if (!TaskHelpers.NotLockedOrExpired("fileLocks", fullPath))
                return;


            if (WatchedFiles.TryGetValue(fullPath, out System.Threading.Timer timer))
            {
                lock (timer)
                    try
                    {
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                        timer.Change(1000, 30000);
                    }
                    catch { }
            }
            else
            {
                timer = null;
            }

            if (timer == null)
            {
                timer = new System.Threading.Timer(TimerCallback, fullPath, 1000, 30000);

                if (!WatchedFiles.TryAdd(fullPath, timer))
                {
                    timer.Dispose();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2002:DoNotLockOnObjectsWithWeakIdentity")]
        void TimerCallback(object state)
        {
            var fullPath = (string)state;

            if (!WatchedFiles.TryGetValue(fullPath, out System.Threading.Timer timer))
                return;

            try
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);

                if (TaskHelpers.NotLockedOrExpired("fileLocks", fullPath))
                {

                    if (WatchedFiles.TryRemove(fullPath, out System.Threading.Timer removedTimer))
                    {
                        lock (removedTimer)
                            try
                            {
                                removedTimer.Change(Timeout.Infinite, Timeout.Infinite);
                                removedTimer.Dispose();
                            }
                            catch { }
                    }

                    base.TriggerEvent(this, fullPath);

                    return;
                }

                timer.Change(1000, 30000);
            }
            catch
            {
                try
                {
                    timer.Change(30000, 30000);
                }
                catch { }
            }
        }

        #region IDisposable Support
        bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Watcher != null)
                    {
                        Watcher.Dispose();
                        Watcher = null;
                    }
                }

                disposedValue = true;
            }
        }
        
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion


    }
}
