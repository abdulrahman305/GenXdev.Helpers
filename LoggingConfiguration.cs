/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using GenXdev.AsyncSockets.Logging;
using GenXdev.Configuration;

namespace GenXdev.AsyncSockets.Configuration
{
    public class LoggingConfiguration : ConfigurationBase, ILoggingConfiguration
    {
        public bool LoggingEnabled { get; set; }

        public int LogHoursFrom { get; set; } = 0;
        public int LogHoursUntil { get; set; } = 23;
        public int HoursToKeep { get; set; } = 2;
        public int MegabytesToKeep { get; set; } = 1000;

        public ServiceLogLevel LogLevel { get; set; } = 0
#if (DEBUG)
            | ServiceLogLevel.LongExceptions
                | ServiceLogLevel.ShortExceptions
                //  | ServiceLogLevel.HandlerFlow
                | ServiceLogLevel.Performance
                | ServiceLogLevel.ProgramFlow
        //| ServiceLogLevel.SocketFlow 
        //  | ServiceLogLevel.ThreadInfo
#else
                //| ServiceLogLevel.LongExceptions
                //| ServiceLogLevel.ShortExceptions
                | ServiceLogLevel.HandlerFlow
                | ServiceLogLevel.Performance
                | ServiceLogLevel.ProgramFlow
        // | ServiceLogLevel.SocketFlow
#endif
        ;
    }
}
