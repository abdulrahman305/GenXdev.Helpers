namespace GenXdev.AsyncSockets.Arguments
{
    public class VersionMismatchEventArgs : EventArgs
    {
        public bool LocalReportsError { get; internal set; }
        public bool RemoteReportedError { get; internal set; }
        public Version LocalVersion { get; internal set; }
        public Version RemoteVersion { get; internal set; }
        public Version MinimumVersionExpected { get; internal set; }
        public Version MinimumVersionRemoteExpects { get; internal set; }
    }
}
