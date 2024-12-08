using Ninject;

namespace GenXdev.AsyncSockets.Containers
{
    public class MPXChannelDataDictionary<TValue> where TValue : MPXChannelData
    {
        Dictionary<string, TValue> Dictionary { get; set; }
        IKernel Kernel;

        public TValue this[string index]
        {
            get
            {
                lock (this)
                {

                    if (!Dictionary.TryGetValue(index, out TValue result))
                    {
                        result = Kernel.Get<TValue>();
                        result.ChannelName = index;
                        Dictionary[index] = result;
                    }

                    return result;
                }
            }

            internal set
            {
                lock (this)
                {
                    if (value == null)
                    {
                        if (Dictionary.ContainsKey(index))
                        {
                            Dictionary[index].Dispose();
                            Dictionary.Remove(index);
                        }
                        return;
                    }

                    if (Dictionary.TryGetValue(index, out TValue b))
                    {
                        if (b != value)
                        {
                            //b.Dispose();
                            Dictionary[index] = value;
                        }
                    }
                    else
                    {
                        Dictionary[index] = value;
                    }
                }
            }
        }

        public TValue[] All
        {
            get
            {
                lock (this)
                {
                    return this.Dictionary.Values.ToArray<TValue>();
                }
            }
        }

        public MPXChannelDataDictionary(IKernel Kernel)
        {
            this.Kernel = Kernel;
            this.Dictionary = new Dictionary<string, TValue>(); ;
        }

        internal void Clear()
        {
            lock (this)
            {
                lock (this)
                {
                    foreach (var data in Dictionary)
                    {
                        data.Value.Dispose();
                    }

                    this.Dictionary.Clear();
                }
            }
        }

        public bool HaveChannel(string channel)
        {
            lock (this)
            {
                return Dictionary.ContainsKey(channel);
            }
        }
    }
}
