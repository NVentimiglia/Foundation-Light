using System;
using System.Collections;
using Foundation.Architecture.Internal;

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
        static readonly IThreadingService Instance = UnityThreadService.Init();
#else
        static readonly IThreadingService Instance = new AsyncThreadService();
#endif
        /// <summary>
        /// Checks if this is the main thread
        /// </summary>
        public static bool IsMainThread
        {
            get { return Instance.IsMainThread; }
        }

        /// <summary>
        /// A long running (continuous) update loop
        /// </summary>
        /// <param name="callback">Update Handler with delta time parameter</param>
        /// <returns></returns>
        public static IDisposable RunUpdate(Action<double> callback)
        {
            return Instance.RunUpdate(callback);
        }

        /// <summary>
        /// Registers a timeout (Wait and Invoke)
        /// </summary>
        public static IDisposable RunDelay(Action callback, float seconds = 5)
        {
            return Instance.RunDelay(callback, seconds);
        }
      
        /// <summary>
        /// A Coroutine. Like an Update Loop, but, execution broken up by yields
        /// </summary>
        public static void RunRoutine(IEnumerator routine)
        {
            Instance.RunRoutine(routine);
        }
        
        /// <summary>
        /// Executes an action on the main thread
        /// </summary>
        public static void RunMainThread(Action action)
        {
            Instance.RunDelay(action);
        }
        
        /// <summary>
        /// Executes an action on the background thread (if possible - webGl)
        /// </summary>
        public static void RunBackgroundThread(Action action)
        {
            Instance.RunBackgroundThread(action);
        }

        /// <summary>
        /// Run a background job with completion
        /// </summary>
        public static void RunBackgroundThread(Action backgroundWork, Action mainWork)
        {
            Instance.RunBackgroundThread(backgroundWork, mainWork);
        }
    }
}
