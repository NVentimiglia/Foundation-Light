// Nicholas Ventimiglia 2016-09-05

using System;

namespace Foundation.Architecture
{
    /// <summary>
    /// Cross platform logging interface
    /// </summary>
    public static class Logger
    {
        /// <summary>
        ///  extensibility point
        /// </summary>
        public static event Action<LogModel> OnLog = delegate { };

        public static void Log(LogModel model)
        {
            switch (model.Level)
            {
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }

            OnLog(model);

            Console.WriteLine(model.Message + (model.Exception == null ? null : model.Exception.Message));
            Console.ResetColor();
        }

        public static void Log(string message)
        {
            Log(new LogModel
            {
                Message = message,
                Level = LogLevel.Info
            });
        }

        public static void LogWarning(string message)
        {
            Log(new LogModel
            {
                Message = message,
                Level = LogLevel.Warning
            });
        }

        public static void LogError(string message)
        {
            Log(new LogModel
            {
                Message = message,
                Level = LogLevel.Error
            });
        }

        public static void LogException(Exception ex)
        {
            Log(new LogModel
            {
                Exception = ex,
                Message = ex.Message,
                Level = LogLevel.Error
            });
        }

        public static void LogException(string message, Exception ex)
        {
            Log(new LogModel
            {
                Exception = ex,
                Message = message,
                Level = LogLevel.Error
            });
        }
    }
}