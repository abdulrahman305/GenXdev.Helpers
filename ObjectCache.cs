/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using Ninject;

namespace GenXdev.Containers
{
    public delegate object ObjectCacheConstructorDelegate();

    public class ObjectCache
    {
        private Stack<object> _items = new Stack<object>();
        private object _sync = new object();
        IKernel Kernel;
        ObjectCacheConstructorDelegate ConstructCallback;

        public ObjectCache(IKernel Kernel)
        {
            this.Kernel = Kernel;
        }

        public ObjectCache(IKernel Kernel, ObjectCacheConstructorDelegate ConstructCallback)
        {
            this.Kernel = Kernel;
            this.ConstructCallback = ConstructCallback;
        }

        internal object _Acquire(Type T)
        {
            lock (_sync)
            {
                if (_items.Count == 0)
                {
                    object newT;
                    newT = Kernel.Get(T);
                    ((CachedObjectBase)newT)._objectPool = this;
                    return newT;
                }
                else
                {
                    return _items.Pop();
                }
            }
        }

        internal object _Acquire()
        {
            lock (_sync)
            {
                if (_items.Count == 0)
                {
                    object newT;
                    newT = ConstructCallback();
                    if (newT is CachedObjectBase)
                        ((CachedObjectBase)newT)._objectPool = this;
                    return newT;
                }
                else
                {
                    return _items.Pop();
                }
            }
        }

        internal protected void _Release(object item)
        {
            lock (_sync)
            {
                _items.Push(item);
            }
        }
    }

    public delegate void ObjectCacheAcquireCallback<T>(T CachedObject);

    public class ObjectCache<T> : ObjectCache where T : CachedObjectBase
    {

        public ObjectCache(IKernel Kernel)
            : base(Kernel)
        {

        }

        public T Acquire()
        {
            T Obj = (T)base._Acquire(typeof(T));

            Obj.InitializeUse();

            return Obj;
        }

        public void Acquire(ObjectCacheAcquireCallback<T> Callback)
        {
            var CachedObject = Acquire();
            try
            {
                Callback(CachedObject);
            }
            finally
            {
                CachedObject.Release();
            }
        }

        internal void Release(T item)
        {
            base._Release(item);
        }
    }
}
