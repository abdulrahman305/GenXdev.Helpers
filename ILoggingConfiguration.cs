using GenXdev.AsyncSockets.Logging;

namespace GenXdev.AsyncSockets.Configuration
{
    public interface ILoggingConfiguration
    {
        bool LoggingEnabled { get; }
        int LogHoursFrom { get; }
        int LogHoursUntil { get; }
        int HoursToKeep { get; }
        int MegabytesToKeep { get; }

        ServiceLogLevel LogLevel { get; }

    }
}
