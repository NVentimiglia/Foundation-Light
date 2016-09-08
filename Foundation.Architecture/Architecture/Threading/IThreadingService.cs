// Nicholas Ventimiglia 2016-09-07

using System;
using System.Collections;

namespace Foundation.Architecture
{
    /// <summary>
    /// Main thread / Background thread utility
    /// </summary>
    /// <remarks>
    /// Games need consistent threading / time logic on server and client.
    /// </remarks>
    public interface IThreadingService
    {
        /// <summary>
        /// A long running (continuous) update loop
        /// </summary>
        /// <param name="callback">Update Handler with delta time parameter</param>
        /// <returns></returns>
        IDisposable RunUpdate(Action<double> callback);
        
        /// <summary>
        /// Registers a timeout (Wait and Invoke)
        /// </summary>
        IDisposable RunDelay(Action callback, int intervalMs = 5000);
        
        /// <summary>
        /// A Coroutine. Like an Update Loop, but, execution broken up by yields
        /// </summary>
        void RunRoutine(IEnumerator routine);
        
        /// <summary>
        /// Executes an action on the main thread
        /// </summary>
        void RunMainThread(Action action);
        
        /// <summary>
        /// Executes an action on the background thread (if possible - webGl)
        /// </summary>
        void RunBackgroundThread(Action action);
    }
}