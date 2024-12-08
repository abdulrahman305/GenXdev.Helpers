namespace GenXdev.Containers
{
    public class ReadonlyDynamicIndexedProperty<Key, Value>
    {
        GetValueCallback<Key, Value> GetValueCallback;
        GetKeysCallback<Key> GetKeysCallback;

        public Value this[Key index]
        {
            get
            {
                return GetValueCallback(index);
            }
        }

        public ReadonlyDynamicIndexedProperty(GetValueCallback<Key, Value> GetValueCallback, GetKeysCallback<Key> GetKeysCallback)
        {
            this.GetValueCallback = GetValueCallback;
            this.GetKeysCallback = GetKeysCallback;
        }
    }
}
