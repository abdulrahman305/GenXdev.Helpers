namespace GenXdev.Configuration
{
    public class OnPropertyChangedEventArgs : EventArgs
    {
        public string PropertyName { get; private set; }
        public object PropertyValue { get; private set; }

        public OnPropertyChangedEventArgs(string PropertyName, object PropertyValue)
        {
            this.PropertyName = PropertyName;
            this.PropertyValue = PropertyValue;
        }
    }
}
