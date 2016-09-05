using System;

namespace Foundation.Architecture
{
    /// <summary>
    /// Service for async processing. Coroutine / task / Update manager
    /// </summary>
    /// <remarks>
    /// For keeping Unity and Server logic similar
    /// </remarks>
    public interface IRoutineService
    {
        /// <summary>
        /// A long running update loop
        /// </summary>
        IDisposable RunLoop(Action<double> callback);
        
        /// <summary>
        /// Registers a timmeout
        /// </summary>
        IDisposable RunTimeout(Action callback, int intervalMs = 5000);

        /// <summary>
        /// Registers a timmeout
        /// </summary>
        IDisposable RunTimeout(Action<object> callback, object state, int intervalMs = 5000);
    }
}
