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
    public static class ThreadingService
    {
#if UNITY
        static readonly IThreadingService Instance ;//= new UnityThreadService();
#else
        static readonly IThreadingService Instance = new AsyncThreadService();
#endif

        /// <summary>
        /// A long running (continuous) update loop
        /// </summary>
        /// <param name="callback">Update Handler with delta time parameter</param>
        /// <returns></returns>
        public static IDisposable RunUpdate(Action<double> callback)
        {
            return Instance.RunUpdate(callback);
        }

        //

        /// <summary>
        /// Registers a timeout (Wait and Invoke)
        /// </summary>
        public static IDisposable RunDelay(Action callback, int intervalMs = 5000)
        {
            return Instance.RunDelay(callback, intervalMs);
        }


        //

        /// <summary>
        /// A Coroutine. Like an Update Loop, but, execution broken up by yields
        /// </summary>
        public static IDisposable RunRoutine(IEnumerator routine)
        {
            return Instance.RunRoutine(routine);
        }

        /// <summary>
        /// A Coroutine. Like an Update Loop, but, execution broken up by yields
        /// </summary>
        public static IDisposable RunRoutine(Func<IEnumerator> routine)
        {
            return Instance.RunRoutine(routine);
        }

        //

        /// <summary>
        /// Executes an action on the main thread
        /// </summary>
        public static void RunMainThread(Action action)
        {
            Instance.RunDelay(action);
        }

        //

        /// <summary>
        /// Executes an action on the background thread (if possible - webGl)
        /// </summary>
        public static void RunBackgroundThread(Action action)
        {
            Instance.RunBackgroundThread(action);
        }

    }
}
