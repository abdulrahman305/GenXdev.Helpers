namespace GenXdev.Events
{
    public class DirectoryUpdateEventHandlerEventArgs : EventArgs
    {
        public DirectoryInfo DirectoryInfo { get; private set; }

        public DirectoryUpdateEventHandlerEventArgs(
            string directoryPath
        )
        {
            this.DirectoryInfo = new DirectoryInfo(directoryPath);
        }
    }
}
