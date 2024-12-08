/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using System.Net.Sockets;

namespace GenXdev.AsyncSockets.Logging
{
    public enum ServiceLogLevel { None = 0, LongExceptions = 1, ShortExceptions = 2, Performance = 4, SocketFlow = 8, HandlerFlow = 32, ProgramFlow = 64 };
    public delegate string LogCallback();

    public interface IServiceLogger
    {
        string LogFilePath { get; }

        void LogException(Exception e, String Message = "");
        void LogSocketHandlerException(Exception e, SocketAsyncEventArgs saea, String Message = "");
        void LogPerformanceStatusMessage(String Message);
        void LogPerformanceStatusMessage(LogCallback callbaack);
        void LogSocketFlowMessage(SocketAsyncEventArgs saea, String Message);
        void LogSocketFlowMessage(SocketAsyncEventArgs saea, LogCallback callback);
        void LogHandlerFlowMessage(SocketAsyncEventArgs saea, String Message);
        void LogHandlerFlowMessage(SocketAsyncEventArgs saea, LogCallback callback);
        void LogProgramFlowInfoMessage(String Message);
        void LogProgramFlowInfoMessage(LogCallback callback);

        void Stop();
        void Start();
    }
}
