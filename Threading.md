# Threading

The threading service is a cross platform service for...

- Doing background work
- Doing work on the main thread from a background thread
- Running coroutines
- Registering 'Unity Style' updates
- Registering 'Unity Style' coroutines
- Registering 'Unity Style' delay invoke

It is cross platform, so you can have time sensitive logic on your client and server running against this common api.

`````
/// <summary>
    /// Main thread / Background thread utility
    /// </summary>
    /// <remarks>
    /// Games need consistent threading / time logic on server and client.
    /// </remarks>
    public static class ThreadingService
    {
        /// <summary>
        /// Checks if this is the main thread
        /// </summary>
        public static bool IsMainThread {get;}

        /// <summary>
        /// A long running (continuous) update loop
        /// </summary>
        /// <param name="callback">Update Handler with delta time parameter</param>
        /// <returns></returns>
        public static IDisposable RunUpdate(Action<double> callback);

        /// <summary>
        /// Registers a timeout (Wait and Invoke)
        /// </summary>
        public static IDisposable RunDelay(Action callback, float seconds = 5);
      
        /// <summary>
        /// A Coroutine. Like an Update Loop, but, execution broken up by yields
        /// </summary>
        public static void RunRoutine(IEnumerator routine);
        
        /// <summary>
        /// Executes an action on the main thread
        /// </summary>
        public static void RunMainThread(Action action);
        
        /// <summary>
        /// Executes an action on the background thread (if possible - webGl)
        /// </summary>
        public static void RunBackgroundThread(Action action);

        /// <summary>
        /// Run a background job with completion
        /// </summary>
        public static void RunBackgroundThread(Action backgroundWork, Action mainWork);
    }
`````
