// Nicholas Ventimiglia 2016-09-05
using System;
using UnityEngine;

namespace Foundation.Architecture
{
    /// <summary>
    /// Unity implementation
    /// </summary>
    public class UnityLogService : ILogService
    {
        public void Log(LogModel model)
        {
            switch (model.Level)
            {
                case LogLevel.Info:
                    Debug.Log(model.Message);
                    break;
                case LogLevel.Error:
                    Debug.LogError(model.Message);
                    if(model.Exception != null)
                        Debug.LogException(model.Exception);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(model.Message);
                    break;
            }
        }

        public void Info(string message)
        {
            Log(new LogModel
            {
                Message = message,
                Level = LogLevel.Info
            });
        }

        public void Warning(string message)
        {
            Log(new LogModel
            {
                Message = message,
                Level = LogLevel.Warning
            });
        }

        public void Error(string message)
        {
            Log(new LogModel
            {
                Message = message,
                Level = LogLevel.Error
            });
        }

        public void Error(Exception ex)
        {
            Log(new LogModel
            {
                Exception = ex,
                Message = ex.Message,
                Level = LogLevel.Error
            });
        }

        public void Error(string message, Exception ex)
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