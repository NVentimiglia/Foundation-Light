using System;

namespace Foundation.Architecture
{
    /// <summary>
    /// Default implementation
    /// </summary>
    public class ConsoleLogService : ILogService
    {
        public void Log(LogModel model)
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

            Console.WriteLine(model.Message + (model.Exception == null ? null : model.Exception.Message));
            Console.ResetColor();
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