// Nicholas Ventimiglia 2016-09-05
using System;

namespace Foundation.Architecture
{
    /// <summary>
    /// Main thread / Background thread utility
    /// </summary>
    public interface IThreadingService
    {
        /// <summary>
        /// A long running update loop
        /// </summary>
        IDisposable RunLoop(Action<double> callback);

        /// <summary>
        /// Registers a timeout
        /// </summary>
        IDisposable RunTimeout(Action callback, int intervalMs = 5000);

        /// <summary>
        /// Registers a timeout
        /// </summary>
        IDisposable RunTimeout(Action<object> callback, object state, int intervalMs = 5000);

        //

        /// <summary>
        /// Executes an action on the main thread
        /// </summary>
        void RunMainThread(Action action);

        /// <summary>
        /// Executes an action on the main thread
        /// </summary>
        void RunMainThread(Action<object> action, object state);

        /// <summary>
        /// Executes an action on the main thread
        /// </summary>
        void RunMainThread<T>(Action<T> action, T state);

        //

        /// <summary>
        /// Executes an action on the background thread (if possible - webGl)
        /// </summary>
        void RunBackgroundThread(Action action);

        /// <summary>
        /// Executes an action on the background thread (if possible - webGl)
        /// </summary>
        void RunBackgroundThread(Action<object> action, object state);

        /// <summary>
        /// Executes an action on the background thread (if possible - webGl)
        /// </summary>
        void RunBackgroundThread<T>(Action<T> action, T state);
    }
}
