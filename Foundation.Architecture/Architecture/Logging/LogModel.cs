// Nicholas Ventimiglia 2016-09-07

using System;

namespace Foundation.Architecture
{
    /// <summary>
    /// Class of log event
    /// </summary>
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// Log information
    /// </summary>
    public struct LogModel
    {
        public LogLevel Level;
        public string Message;
        public Exception Exception;
    }
}