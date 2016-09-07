// Nicholas Ventimiglia 2016-09-05
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

        //

        /// <summary>
        /// Registers a timeout (Wait and Invoke)
        /// </summary>
        IDisposable RunDelay(Action callback, int intervalMs = 5000);

        /// <summary>
        /// Registers a timeout (Wait and Invoke)
        /// </summary>
        IDisposable RunDelay(Action<object> callback, object state, int intervalMs = 5000);

        /// <summary>
        /// Registers a timeout (Wait and Invoke)
        /// </summary>
        IDisposable RunDelay<TState>(Action<TState> callback, TState state, int intervalMs = 5000);

        //

        /// <summary>
        /// A Coroutine. Like an Update Loop, but, execution broken up by yields
        /// </summary>
        IDisposable RunRoutine(IEnumerator routine);

        /// <summary>
        /// A Coroutine. Like an Update Loop, but, execution broken up by yields
        /// </summary>
        IDisposable RunRoutine(Func<IEnumerator> routine);

        /// <summary>
        /// A Coroutine. Like an Update Loop, but, execution broken up by yields
        /// </summary>
        IDisposable RunRoutine(Func<object, IEnumerator> routine, object state);

        /// <summary>
        /// A Coroutine. Like an Update Loop, but, execution broken up by yields
        /// </summary>
        IDisposable RunRoutine<TState>(Func<TState, IEnumerator> routine, TState state);
     
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
        void RunMainThread<TState>(Action<TState> action, TState state);

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
        void RunBackgroundThread<TState>(Action<TState> action, TState state);
    }
}
