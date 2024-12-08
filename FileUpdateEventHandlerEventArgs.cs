namespace GenXdev.Events
{
    public class FileUpdateEventHandlerEventArgs : EventArgs
    {
        public FileInfo FileInfo { get; private set; }

        public FileUpdateEventHandlerEventArgs(
            string filePath
        )
        {
            this.FileInfo = new FileInfo(filePath);
        }
    }
}
