using GenXdev.AsyncSockets.Configuration;
using GenXdev.AsyncSockets.Logging;
using GenXdev.Buffers;
using GenXdev.Helpers;
using Ninject;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using GenXdev.Misc;

namespace GenXdev.AsyncSockets.Handlers
{
    public class ApplicationLogger : IServiceLogger
    {
        static int MAX_LOG_FILE_SIZE = (1024 * 1024 * 10);
#if (Logging)
        static long Sequence = 0;
#endif
        static long LastHour = System.DateTime.UtcNow.Hour;
        public object SyncRoot { get; private set; }
        UTF8Encoding utf8 = new UTF8Encoding(false);
        GenXdev.Additional.HiPerfTimer.NativeMethods StopWatch = new GenXdev.Additional.HiPerfTimer.NativeMethods(true);
        GenXdev.Additional.HiPerfTimer.NativeMethods StopWatch2 = new GenXdev.Additional.HiPerfTimer.NativeMethods(true);
        ILoggingConfiguration Config;
        //#if (Release)
        //        bool LogToConsole = false;
        //#else
        bool LogToConsole =
#if (Debug)
            true;
#else
            Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Contains("console") ||
            Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Contains("test");
#endif

        //#endif
        System.DateTime LogStartTime = System.DateTime.UtcNow;
        DynamicBuffer BufferMain;
        DynamicBuffer BufferDump;
        public string LogFilePath { get; private set; }

        [Inject]
        public ApplicationLogger(IKernel Kernel, ILoggingConfiguration Config, string LogFilePath = null)
        {
            // this instance may only be used once
            OnlyOneInstanceChecker.Check(this.GetType());

            this.Config = Config;

            this.BufferMain = Kernel.Get<DynamicBuffer>();
            this.BufferDump = Kernel.Get<DynamicBuffer>();
            this.SyncRoot = new object();
            this.LogFilePath = Path.Combine(GenXdev.Helpers.Environment.GetApplicationRootDirectory(), Path.GetFileNameWithoutExtension(GenXdev.Helpers.Environment.GetEntryAssemblyLocation()) + ".log");

            if (!String.IsNullOrWhiteSpace(LogFilePath))
            {
                if (Path.IsPathRooted(LogFilePath))
                {
                    if (Directory.Exists(LogFilePath))
                    {
                        this.LogFilePath = Path.Combine(LogFilePath, Path.GetFileName(this.LogFilePath));
                    }
                    else
                    {
                        this.LogFilePath = LogFilePath;
                    }
                }
                else
                {
                    this.LogFilePath = Path.Combine(Path.GetDirectoryName(this.LogFilePath), LogFilePath);
                }
            }

            var directory = Path.GetDirectoryName(this.LogFilePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            else
            {
                DeleteOldLogFiles();
            }
        }

        System.DateTime lastMaintenance = System.DateTime.MinValue;
        long state = 0;
#if (Logging)
        volatile bool Stopped;
#endif

        public void Stop()
        {
            StopWatch.Stop();
#if (Logging)
            Stopped = true;
#endif
            BufferMain.MoveTo(BufferDump);
            ProcessLog(false);
        }

        public void Start()
        {
            StopWatch.Start();
#if (Logging)
            Stopped = false;
#endif
        }

        void ProcessLog(object ignore)
        {
            System.DateTime logDate;
            lock (BufferMain.SyncRoot)
            {
                logDate = LogStartTime;
                LogStartTime = System.DateTime.UtcNow;
            }
            try
            {
                BufferDump.Position = 0;
                var filename =
                    Path.Combine(

                    Path.GetDirectoryName(LogFilePath),
                    logDate.Year.ToString()
                    + logDate.Month.ToString().PadLeft(2, '0')
                    + logDate.Day.ToString().PadLeft(2, '0')
                    + logDate.Hour.ToString().PadLeft(2, '0')
                    + "_"
                    + Path.GetFileName(LogFilePath)
                    );

                using (var fs = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    BufferDump.Position = 0;
                    BufferDump.WriteToStream(fs);
                }
                BufferDump.Reset();

                DeleteOldLogFiles();
            }
            catch { }
            finally
            {
                Interlocked.Exchange(ref state, 0);
            }
        }

        private bool LoggingHoursOk(System.DateTime now)
        {
            return now.Hour >= Config.LogHoursFrom &&
                   now.Hour <= Config.LogHoursUntil;
        }

        private bool ShouldDumpLogfile(System.DateTime now)
        {
            long prevHour = (now.Hour + 23) % 24;
            return (BufferMain.Count > MAX_LOG_FILE_SIZE * 2) || (this.StopWatch2.Duration > 60) ||
                    Interlocked.CompareExchange(ref LastHour, now.Hour, prevHour) == prevHour;
        }

        private void DeleteOldLogFiles()
        {
            var now = System.DateTime.UtcNow;
            now = new System.DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, System.DateTimeKind.Utc);

            if (!LoggingHoursOk(now)) return;

            var logFiles = (
                from q in
                    Directory.EnumerateFiles(Path.GetDirectoryName(LogFilePath))
                orderby
                    Path.GetFileName(q) descending
                select q
            ).ToList<String>();

            long spaceUsed = 0;

            foreach (var logFile in logFiles)
            {
                var fn = Path.GetFileName(logFile);

                if (!logFile.EndsWith("_" + Path.GetFileName(LogFilePath))) continue;

                try
                {
                    var date = new System.DateTime(
                        Int32.Parse(fn.Substring(0, 4)),
                        Int32.Parse(fn.Substring(4, 2)),
                        Int32.Parse(fn.Substring(6, 2)),
                        Int32.Parse(fn.Substring(8, 2)),
                        0,
                        0,
                        System.DateTimeKind.Utc
                    );

                    var fileSize = new FileInfo(logFile).Length;
                    var diskQuotaOverrun = spaceUsed + fileSize > Config.MegabytesToKeep * 1000000;

                    var ageOfLogFile = now - date;
                    var fileIsToOld = Config.HoursToKeep > 0 && ageOfLogFile.TotalHours >= Config.HoursToKeep;

                    if (fileIsToOld || diskQuotaOverrun)
                    {
                        if (LogToConsole)
                        {

                            Console.WriteLine("Deleting logfile " + fn + " " +
                                (fileIsToOld ? "to old (" + ageOfLogFile.TotalHours.ToString() + " hours old. " : "") +
                                (diskQuotaOverrun ? "disk quota overrun: " + Math.Round((spaceUsed + fileSize) / 1000000d, 1).ToString() + " MB" : "")
                            );
                        }
                        GenXdev.Helpers.FileSystem.ForciblyDeleteFile(logFile, false);
                    }
                    else
                    {
                        spaceUsed += fileSize;
                    }
                }
                catch (Exception e)
                {
                    if (LogToConsole) Console.WriteLine("Error processing log file " + fn + " : " + e.Message);
                }
            }
        }

        public void Log(String Message)
        {
#if (Logging)

            var now = System.DateTime.UtcNow;

            if (Stopped || String.IsNullOrWhiteSpace(Message))
                return;

            Message = Message.Trim();
            Message = Message[0] + Message.Substring(1);
            Message = "#" + Interlocked.Increment(ref Sequence).ToString().PadLeft(10) + " | " +
               now.ToString() + " | " + Message + "\r\n";

            if (LoggingHoursOk(now))
                lock (BufferMain.SyncRoot)
                {
                    if (ShouldDumpLogfile(now))
                    {
                        if (Interlocked.CompareExchange(ref state, 1, 0) == 0)
                        {
                            BufferDump.Reset();
                            BufferMain.MoveTo(BufferDump);
                            this.StopWatch2.Reset();

                            if (!ThreadPool.QueueUserWorkItem(ProcessLog))
                            {
                                ProcessLog(null);
                            }
                        }
                    }

                    BufferMain.Add(utf8.GetBytes(Message));
                }
#if (DEBUG)
            System.Diagnostics.Debug.Write(Message);
#endif
            if (LogToConsole)
            {
                int i = Message.IndexOf(" =>");
                if (i > 0)
                {
                    Console.WriteLine(Message.Substring(0, i));
                }
                else
                {
                    Console.Write(Message);
                }
            }
#endif
        }

        public void LogException(Exception e, String Message = "")
        {
#if (Logging)
            if (!(LogToConsole || Config.LoggingEnabled))
                return;
            try
            {
                Message = (Message == null ? "" : Message) + "\r\n" + e.StackTrace;
            }
            catch { }
            try
            {
                if (((int)Config.LogLevel & (int)ServiceLogLevel.LongExceptions) > 0)
                {
                    Log(StopWatch.Duration.ToString("0.00000").PadLeft(10) + " | " + (String.IsNullOrWhiteSpace(Message) ? "" : Message + " | ") + e.ToLogString(Message));
                }
                else
                    if (((int)Config.LogLevel & (int)ServiceLogLevel.ShortExceptions) > 0)
                {
                    Log(StopWatch.Duration.ToString("0.00000").PadLeft(10) + " | " + (String.IsNullOrWhiteSpace(Message) ? "" : Message + " | ") + e.Message);
                }
            }
            catch { }
#endif
        }

        public void LogSocketHandlerException(Exception e, SocketAsyncEventArgs saea, String Message = "")
        {
#if (Logging)
            if (!(LogToConsole || Config.LoggingEnabled))
                return;
            try
            {
                SocketHandlerBase controller = (SocketHandlerBase)saea.Handler();
                string cn = controller.GetType().ToString();
                int i = cn.LastIndexOf('.');
                if ((i > 0) && (i < cn.Length - 1))
                    cn = cn.Substring(i + 1);
                if (controller != null)
                {
                    cn += "#" + controller.UniqueSocketId;
                }

                if (((int)Config.LogLevel & (int)ServiceLogLevel.LongExceptions) > 0)
                {
                    Log(StopWatch.Duration.ToString("0.00000").PadLeft(10) + " | " + cn + " | " + (String.IsNullOrWhiteSpace(Message) ? "" : Message + " | ") + e.ToLogString(Message));
                }
                else
                    if (((int)Config.LogLevel & (int)ServiceLogLevel.ShortExceptions) > 0)
                {
                    Log(StopWatch.Duration.ToString("0.00000").PadLeft(10) + " | " + cn + "  | " + (String.IsNullOrWhiteSpace(Message) ? "" : Message + " | ") + e.Message);
                }
            }
            catch { }
#endif
        }

        public void LogPerformanceStatusMessage(String Message)
        {
#if (Logging)
            if (!(LogToConsole || Config.LoggingEnabled))
                return;
            try
            {
                if (((int)Config.LogLevel & (int)ServiceLogLevel.Performance) > 0)
                {
                    Log(Message);
                }
            }
            catch { }
#endif
        }

        public void LogPerformanceStatusMessage(LogCallback callback)
        {
#if (Logging)
            if (!(LogToConsole || Config.LoggingEnabled))
                return;
            try
            {
                if (((int)Config.LogLevel & (int)ServiceLogLevel.Performance) > 0)
                {
                    Log(callback());
                }
            }
            catch { }
#endif
        }


        public void LogSocketFlowMessage(SocketAsyncEventArgs saea, String Message)
        {
#if (Logging)
            if (!(LogToConsole || Config.LoggingEnabled))
                return;
            try
            {
                if (((int)Config.LogLevel & (int)ServiceLogLevel.SocketFlow) > 0)
                {
                    SocketHandlerBase controller = (SocketHandlerBase)saea.Handler();
                    string cn = "";
                    if (controller != null)
                    {
                        cn = controller.GetType().ToString();
                        int i = cn.LastIndexOf('.');
                        if ((i > 0) && (i < cn.Length - 1))
                            cn = cn.Substring(i + 1);
                        cn += " | h_" + controller.UniqueSocketId;
                    }
                    Log(StopWatch.Duration.ToString("0.00000").PadLeft(10) + " | " + cn + " | " + Message);
                }
            }
            catch { }
#endif
        }

        public void LogSocketFlowMessage(SocketAsyncEventArgs saea, LogCallback callback)
        {
#if (Logging)
            if (!(LogToConsole || Config.LoggingEnabled))
                return;
            try
            {
                if (((int)Config.LogLevel & (int)ServiceLogLevel.SocketFlow) > 0)
                {
                    SocketHandlerBase controller = (SocketHandlerBase)saea.Handler();
                    string cn = "";
                    if (controller != null)
                    {
                        cn = controller.GetType().ToString();
                        int i = cn.LastIndexOf('.');
                        if ((i > 0) && (i < cn.Length - 1))
                            cn = cn.Substring(i + 1);
                        cn += " | h_" + controller.UniqueSocketId;
                    }
                    Log(StopWatch.Duration.ToString("0.00000").PadLeft(10) + " | " + cn + " | " + callback());
                }
            }
            catch { }
#endif
        }

        public void LogHandlerFlowMessage(SocketAsyncEventArgs saea, String Message)
        {
#if (Logging)
            if (!(LogToConsole || Config.LoggingEnabled))
                return;
            try
            {
                if (((int)Config.LogLevel & (int)ServiceLogLevel.HandlerFlow) > 0)
                {
                    SocketHandlerBase controller = (SocketHandlerBase)saea.Handler();
                    string cn = "";
                    if (controller != null)
                    {
                        cn = controller.GetType().ToString();
                        int i = cn.LastIndexOf('.');
                        if ((i > 0) && (i < cn.Length - 1))
                            cn = cn.Substring(i + 1);
                        cn += " | h_" + controller.UniqueSocketId;
                    }


                    Log(StopWatch.Duration.ToString("0.00000").PadLeft(10) + " | " + cn + " | " + Message);
                }
            }
            catch { }
#endif
        }
        public void LogHandlerFlowMessage(SocketAsyncEventArgs saea, LogCallback callback)
        {
#if (Logging)
            if (!(LogToConsole || Config.LoggingEnabled))
                return;
            try
            {
                if (((int)Config.LogLevel & (int)ServiceLogLevel.HandlerFlow) > 0)
                {

                    SocketHandlerBase controller = (SocketHandlerBase)saea.Handler();
                    string cn = "";
                    if (controller != null)
                    {
                        cn = controller.GetType().ToString();
                        int i = cn.LastIndexOf('.');
                        if ((i > 0) && (i < cn.Length - 1))
                            cn = cn.Substring(i + 1);
                        cn += " | h_" + controller.UniqueSocketId;
                    }

                    Log(StopWatch.Duration.ToString("0.00000").PadLeft(10) + " | " + cn + " | " + callback());
                }
            }
            catch { }
#endif
        }
        public void LogProgramFlowInfoMessage(String Message)
        {
#if (Logging)
            if (!(LogToConsole || Config.LoggingEnabled))
                return;
            try
            {
                if (((int)Config.LogLevel & (int)ServiceLogLevel.ProgramFlow) > 0)
                {
                    Log(StopWatch.Duration.ToString("0.00000").PadLeft(10) + " | " + Message);
                }
            }
            catch { }
#endif
        }

        public void LogProgramFlowInfoMessage(LogCallback callback)
        {
#if (Logging)
            if (!(LogToConsole || Config.LoggingEnabled))
                return;
            try
            {
                if (((int)Config.LogLevel & (int)ServiceLogLevel.ProgramFlow) > 0)
                {
                    Log(StopWatch.Duration.ToString("0.00000").PadLeft(10) + " | " + callback());
                }
            }
            catch { }
#endif
        }
    }
}
