namespace GenXdev.Helpers
{
    public static class TaskHelpers
    {
        internal static Dictionary<string, Dictionary<string, TaskInfo>> tasks = new Dictionary<string, Dictionary<string, TaskInfo>>();

        public static bool TryLockIfExpired(string taskGroup, string taskName, out TaskInfo taskInfo)
        {
            taskInfo = FindOrCreateTask(taskGroup, taskName);

            return taskInfo.TryLockIfExpired();
        }

        public static bool TryLock(string taskGroup, string taskName, out TaskInfo taskInfo)
        {
            taskInfo = FindOrCreateTask(taskGroup, taskName);

            return taskInfo.TryLock();
        }

        public static bool NotLockedOrExpired(string taskGroup, string taskName, TimeSpan? expirationTime = null, bool force = false)
        {
            var taskInfo = FindOrCreateTask(taskGroup, taskName);
            bool result = false;
            try
            {
                if (force)
                {
                    result = taskInfo.TryLock();
                }
                else
                {
                    result = taskInfo.TryLockIfExpired();
                }
            }
            finally
            {
                if (result)
                    taskInfo.UnLock(expirationTime);
            }

            return result;
        }

        /// <summary>
        /// if successfull and expirationTime is set, it will fail for that duration afterwards
        /// </summary>
        public static bool HadRecentActivity(
            string taskGroup, string taskName, TimeSpan? expirationTime)
        {
            var taskInfo = FindOrCreateTask(taskGroup, taskName);

            if (taskInfo.TryLockIfExpired())
                try
                {
                    return true;
                }
                finally
                {
                    taskInfo.UnLock(expirationTime);
                }

            return false;
        }
        /// <summary>
        /// if successfull and expirationTime is set, it will fail for that duration afterwards
        /// </summary>
        public static void ClearOutCurrentlyActiveExpiration(string taskGroup, string taskName)
        {
            var taskInfo = FindOrCreateTask(taskGroup, taskName);
            taskInfo.UnLock();
        }

        public static IEnumerable<TaskInfo> GetExpiredTasks(string GroupName)
        {
            return new ExpiredTaskEnumerable(GroupName);
        }

        static long lastTimestamp = System.DateTime.UtcNow.Ticks;

        private static TaskInfo FindOrCreateTask(string taskGroup, string taskName)
        {
            TaskInfo taskInfo;
            lock (tasks)
            {
                long now = System.DateTime.UtcNow.Ticks;

                if (now < lastTimestamp)
                {
                    tasks.Clear();
                }

                lastTimestamp = now;

                // find or create group

                if (!tasks.TryGetValue(taskGroup, out Dictionary<string, TaskInfo> foundGroup))
                {
                    foundGroup = new Dictionary<string, TaskInfo>();

                    tasks.Add(taskGroup, foundGroup);
                }

                // find task
                if (!foundGroup.TryGetValue(taskName, out taskInfo))
                {
                    taskInfo = new TaskInfo()
                    {
                        GroupName = taskGroup,
                        TaskName = taskName
                    };

                    foundGroup.Add(taskName, taskInfo);
                }
            }
            return taskInfo;
        }
    }

    public class TaskInfo
    {
        public TaskInfo()
        {
            Properties = new Dictionary<string, object>();

            Expires = System.DateTime.MinValue;
        }

        internal long state;

        public Dictionary<string, object> Properties { get; private set; }

        public string GroupName { get; internal set; }
        public string TaskName { get; internal set; }

        public System.DateTime Expires
        {
            get
            {
                lock (this)
                {
                    return _Expires;
                }
            }

            internal set
            {
                lock (this)
                {
                    _Expires = value;
                }
            }
        }

        System.DateTime _Expires;

        public bool TryLock()
        {
            return Interlocked.CompareExchange(ref state, 1, 0) == 0;
        }

        public bool TryLockIfExpired()
        {
            lock (this)
            {
                if (IsExpired())
                {
                    return Interlocked.CompareExchange(ref state, 1, 0) == 0;
                }

                return false;
            }
        }

        public bool IsExpired()
        {
            lock (this)
                return System.DateTime.UtcNow >= Expires;
        }

        public void UnLock(TimeSpan? expires = null)
        {
            if (Interlocked.CompareExchange(ref state, 2, 1) == 1)
            {
                if (expires.HasValue)
                {
                    lock (this)
                    {
                        Expires = System.DateTime.UtcNow + expires.Value;
                        Interlocked.Exchange(ref state, 0);
                    }
                }
                else
                {
                    lock (TaskHelpers.tasks)
                    {
                        // find group
                        if (TaskHelpers.tasks.TryGetValue(GroupName, out Dictionary<string, TaskInfo> foundGroup))
                        {
                            foundGroup.Remove(TaskName);

                            if (foundGroup.Count == 0)
                            {
                                TaskHelpers.tasks.Remove(GroupName);
                            }
                        }
                    }
                }
            }
        }
    }
}
