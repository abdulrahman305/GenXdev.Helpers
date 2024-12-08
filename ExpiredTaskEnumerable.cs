namespace GenXdev.Helpers
{
    public class ExpiredTaskEnumerable : IEnumerable<TaskInfo>
    {
        string GroupName;

        public ExpiredTaskEnumerable(string groupName)
        {
            this.GroupName = groupName;
        }

        public IEnumerator<TaskInfo> GetEnumerator()
        {
            return new ExpiredTaskEnumerator(GroupName);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new ExpiredTaskEnumerator(GroupName);
        }
    }
}
