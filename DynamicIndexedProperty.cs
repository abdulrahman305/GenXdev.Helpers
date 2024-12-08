namespace GenXdev.Containers
{
    public delegate Value GetValueCallback<Key, Value>(Key Key);
    public delegate void SetValueCallback<Key, Value>(Key Key, Value Value);
    public delegate Key[] GetKeysCallback<Key>();
    public class DynamicIndexedProperty<Key, Value>
    {
        GetValueCallback<Key, Value> GetValueCallback;
        SetValueCallback<Key, Value> SetValueCallback;
        GetKeysCallback<Key> GetKeysCallback;

        public Value this[Key index]
        {
            get
            {
                return GetValueCallback(index);
            }

            set
            {
                SetValueCallback(index, value);
            }
        }

        public DynamicIndexedProperty(
            GetValueCallback<Key, Value> GetValueCallback,
            SetValueCallback<Key, Value> SetValueCallback,
            GetKeysCallback<Key> GetKeysCallback)
        {
            this.GetValueCallback = GetValueCallback;
            this.SetValueCallback = SetValueCallback;
            this.GetKeysCallback = GetKeysCallback;
        }
    }
}
