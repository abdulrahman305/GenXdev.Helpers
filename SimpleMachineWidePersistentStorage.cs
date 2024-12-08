namespace GenXdev.Helpers
{
    public class SimpleMachineWidePersistentStorage<T> where T : class
    {
        public SimpleMachineWidePersistentStorage(string name, T defaultValue)
        {
            FilePath = Path.Combine(
                GenXdev.Helpers.Environment.GetApplicationRootDirectory(),
                name + ".json"
            );

            loadFromFile(defaultValue);
        }

        public string FilePath { get; private set; }

        public System.DateTime? LastFileTimeStamp { get; private set; }

        private T _Current = default(T);

        public T Data
        {
            get
            {
                loadFromFile();

                return _Current;
            }

            set
            {
                _Current = value;

                saveToFile();
            }
        }
        public void loadFromFile(T defaultValue = default(T))
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var fileInfo = new FileInfo(FilePath);

                    if (!LastFileTimeStamp.HasValue || LastFileTimeStamp.Value != fileInfo.LastWriteTimeUtc)
                    {
                        int retryCount = 0;

                        while (true) try
                            {
                                _Current = GenXdev.Helpers.Serialization.FromJson<T>(File.ReadAllText(FilePath));

                                LastFileTimeStamp = fileInfo.LastWriteTimeUtc;

                                return;
                            }
                            catch (Exception e)
                            {
                                if (++retryCount == 10)
                                {
                                    throw e;
                                }

                                if (defaultValue != default(T))
                                {
                                    _Current = defaultValue;
                                }

                                System.Threading.Thread.Sleep(100);
                            }
                    }
                }
                else
                {
                    if (defaultValue != default(T))
                    {
                        _Current = defaultValue;
                    }
                }
            }
            catch
            {

            }
        }

        public void saveToFile()
        {
            try
            {
                int retryCount = 0;

                while (true) try
                    {
                        if (GenXdev.Helpers.Serialization.ToJsonFile(_Current, FilePath, false))
                        {
                            return;
                        }

                        if (++retryCount == 10)
                        {
                            throw new Exception("Could not write to " + FilePath);
                        }

                        System.Threading.Thread.Sleep(100);
                    }
                    catch (Exception e)
                    {
                        if (++retryCount == 10)
                        {
                            throw e;
                        }

                        System.Threading.Thread.Sleep(100);
                    }
            }
            catch
            {

            }
        }
    }
}
