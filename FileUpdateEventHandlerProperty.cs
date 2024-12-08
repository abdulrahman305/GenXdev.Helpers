using GenXdev.Helpers;

namespace GenXdev.Events
{
    public class FileUpdateEventHandlerProperty
    {
        public event EventHandler<FileUpdateEventHandlerEventArgs> OnFileAvailableAfterChange;

        protected virtual void TriggerFileEvent(object sender, string filePath)
        {
            try
            {
                var handler = OnFileAvailableAfterChange;

                if (handler != null)
                {
                    var args = new FileUpdateEventHandlerEventArgs(filePath);

                    handler(sender, args);
                }
            }
            catch { }
        }

        public event EventHandler<DirectoryUpdateEventHandlerEventArgs> OnDirectoryAvailableAfterChange;

        protected virtual void TriggerDirectoryEvent(object sender, string directoryPath)
        {
            try
            {
                var handler = OnDirectoryAvailableAfterChange;

                if (handler != null)
                {
                    var args = new DirectoryUpdateEventHandlerEventArgs(directoryPath);

                    handler(sender, args);
                }
            }
            catch { }
        }


        public string Directory { get; private set; }
        public string SearchMask { get; private set; }
        public bool IncludeSubdirectories { get; private set; }

        public FileUpdateEventHandlerProperty(
            string directory,
            string searchMask,
            bool includeSubDirectories
        )
        {
            this.Directory = directory;
            this.SearchMask = searchMask;
            this.IncludeSubdirectories = includeSubDirectories;
        }

        public void TriggerEvent(object sender, string filePath)
        {
            try
            {
                filePath = Path.GetFullPath(filePath);

                var dirExists = System.IO.Directory.Exists(filePath);
                var fileExists = System.IO.File.Exists(filePath);

                var dirTriggered = dirExists ? Path.GetFullPath(filePath) : Path.GetFullPath(Path.GetDirectoryName(filePath));
                var dirWatching = Path.GetFullPath(Directory);

                if (dirTriggered != dirWatching)
                {
                    if (IncludeSubdirectories)
                    {
                        // is it not a subdirectory?
                        if (dirTriggered.IndexOf(dirWatching + "\\") < 0)
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                var filenameTriggered = Path.GetFileName(filePath);

                if (!fileExists && !dirExists)
                    return;

                if (!GenXdev.Helpers.FileSystem.FileNameFitsMask(filenameTriggered, this.SearchMask))
                    return;

                if (dirExists)
                {
                    TriggerDirectoryEvent(sender, filePath);
                }
                else
                {
                    TriggerFileEvent(sender, filePath);
                }
            }
            catch { }
        }
    }
}
