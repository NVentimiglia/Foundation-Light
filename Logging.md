# Logging

LogService is a light weight wrapper around Console.WriteLine and Debug.Log. It is inteded a a cross platform logging service that can be called on shared code.

````
  /// <summary>
    /// Cross platform logging interface
    /// </summary>
    public class LogService
    {
        /// <summary>
        ///  extensibility point
        /// </summary>
        public static event Action<LogModel> OnLog = delegate { };

        public static void Log(LogModel model);

        public static void Log(string message);

        public static void LogWarning(string message);

        public static void LogError(string message);

        public static void LogException(Exception ex);
        
        public static void LogException(string message, Exception ex);
      }
````
