// Nicholas Ventimiglia 2016-09-05
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

    /// <summary>
    /// Service for reporting client information
    /// </summary>
    interface ILogService
    {
        /// <summary>
        /// Log this
        /// </summary>
        void Log(LogModel model);

        /// <summary>
        /// General information
        /// </summary>
        void Info(string message);

        /// <summary>
        /// Potentially a problem
        /// </summary>
        void Warning(string message);

        /// <summary>
        /// Defiantly a problem
        /// </summary>
        void Error(string message);

        /// <summary>
        /// Defiantly a problem
        /// </summary>
        void Error(Exception ex);

        /// <summary>
        /// Defiantly a problem
        /// </summary>
        void Error(string message, Exception ex);
    }
}