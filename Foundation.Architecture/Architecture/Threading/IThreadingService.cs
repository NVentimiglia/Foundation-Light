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

        /// <summary>
        /// Registers a timeout
        /// </summary>
        IDisposable RunTimeout<TState>(Action<TState> callback, TState state, int intervalMs = 5000);

        //

        /// <summary>
        /// A Coroutine
        /// </summary>
        IDisposable RunRoutine(IEnumerator routine);

        /// <summary>
        /// A Coroutine
        /// </summary>
        IDisposable RunRoutine(Func<IEnumerator> routine);

        /// <summary>
        /// A Coroutine
        /// </summary>
        IDisposable RunRoutine(Func<object, IEnumerator> routine, object state);

        /// <summary>
        /// A Coroutine
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
