// Nicholas Ventimiglia 2016-09-05

using System;
using UnityEngine;

namespace Foundation.Architecture
{
    /// <summary>
    /// Cross platform logging interface
    /// </summary>
    public class Logger
    {
        /// <summary>
        ///  extensibility point
        /// </summary>
        public static event Action<LogModel> OnLog = delegate { };

        public static void Log(LogModel model)
        {
            switch (model.Level)
            {
                case LogLevel.Info:
                    Debug.Log(model.Message);
                    break;
                case LogLevel.Error:
                    Debug.LogError(model.Message);
                    if (model.Exception != null)
                        Debug.LogException(model.Exception);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(model.Message);
                    break;
            }

            OnLog(model);
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