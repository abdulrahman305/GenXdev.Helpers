namespace GenXdev.Helpers
{
    public class ExpiredTaskEnumerator : IEnumerator<TaskInfo>
    {
        string GroupName;
        TaskInfo[] Tasks;
        int CurrentIndex;

        public ExpiredTaskEnumerator(string groupName)
        {
            this.GroupName = groupName;
        }

        public TaskInfo Current
        {
            get
            {
                if ((CurrentIndex >= 0) && (CurrentIndex < Tasks.Length))
                {
                    return Tasks[CurrentIndex];
                }

                return null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ~ExpiredTaskEnumerator()
        {
            Dispose(false);
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                if ((CurrentIndex >= 0) && (CurrentIndex < Tasks.Length))
                {
                    return Tasks[CurrentIndex];
                }

                return null;
            }
        }

        public bool MoveNext()
        {
            do
            {
                CurrentIndex++;

                if (CurrentIndex < Tasks.Length)
                {
                    var task = Tasks[CurrentIndex];

                    if (task.TryLockIfExpired())
                    {
                        return true;
                    }
                }
                else
                    return false;

            } while (true);
        }

        public void Reset()
        {
            lock (TaskHelpers.tasks)
            {
                // find group
                if (TaskHelpers.tasks.TryGetValue(GroupName, out Dictionary<string, TaskInfo> foundGroup))
                {
                    this.Tasks = (from q in foundGroup.Values select q).ToArray<TaskInfo>();
                }

                this.Tasks = new TaskInfo[0];
                this.CurrentIndex = -1;
            }
        }
    }
}
