namespace GenXdev.Containers
{
    public class CachedObjectBase
    {
        internal ObjectCache _objectPool { get; set; }

        public void Release()
        {
            FinalizeUse();

            _objectPool._Release(this);
        }

        protected internal virtual void InitializeUse()
        {
        }

        protected virtual void FinalizeUse()
        {
        }
    }
}
