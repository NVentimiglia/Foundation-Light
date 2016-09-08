using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
#if USE_THREAD
using System.Threading;
#endif

namespace Foundation.Architecture.Internal
{
    public class UnityThreadService : MonoBehaviour, IThreadingService
    {
        #region Defines
        public class UpdateTask : IDisposable
        {
            public Action<double> Action;
            public Action<UpdateTask> Cleanup;

            public bool IsDisposed;

            public void Dispose()
            {
                Cleanup(this);
                Cleanup = null;
                Action = null;
                IsDisposed = true;
                //Note : for pooling confirm not used by client and controller. Double Dispose ?
            }
        }

        public class RoutineTask : IDisposable
        {
            public IEnumerator Routine;

            public bool IsDisposed;

            public void Dispose()
            {
                Routine = null;
                IsDisposed = true;
                //Note : for pooling confirm not used by client and controller. Double Dispose ?
            }
        }

        public class JobTask
        {
            public Action Background;
            public Action Main;
        }
        #endregion

        #region API
        public IDisposable RunUpdate(Action<double> callback)
        {
            var task = new UpdateTask { Cleanup = Remove };
            task.Action = callback;

            lock (pendingUpdates)
            {
                pendingUpdates.Add(task);
                hasUpdates = true;
            }

            return task;
        }

        public IDisposable RunDelay(Action callback, float seconds = 5)
        {
            var task = new RoutineTask();
            task.Routine = RunDelayAsync(callback, seconds, task);

            lock (pendingRoutine)
            {
                pendingRoutine.Enqueue(task);
                hasRoutine = true;
            }

            return task;
        }

        public void RunRoutine(IEnumerator routine)
        {
            var task = new RoutineTask();
            task.Routine = routine;

            lock (pendingRoutine)
            {
                pendingRoutine.Enqueue(task);
                hasRoutine = true;
            }
        }

        public void RunMainThread(Action action)
        {
#if USE_THREAD
            lock (pendingMain)
            {
                pendingMain.Enqueue(action);
                hasMain = true;
            }
#else
            action();
#endif
        }

        public void RunBackgroundThread(Action action)
        {
#if USE_THREAD
            lock (pendingBack)
            {
                pendingBack.Enqueue(new JobTask { Background = action });
                hasBack = true;
            }
#else
            action();
#endif
        }

        public void RunBackgroundThread(Action bgAction, Action mainAction)
        {
#if USE_THREAD
            lock (pendingBack)
            {
                pendingBack.Enqueue(new JobTask { Background = bgAction, Main = mainAction });
                hasBack = true;
            }
#else
            bgAction();
            mainAction();
#endif
        }
        #endregion

        #region Implementation

        private static UnityThreadService _instance;
        //[RuntimeInitializeOnLoadMethod]
        public static UnityThreadService Init()
        {
            if (_instance == null)
            {
                var go = new GameObject("_UnityThreadService");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<UnityThreadService>();
            }
            return _instance;
        }

        private readonly List<UpdateTask> pendingUpdates = new List<UpdateTask>();
        private readonly Queue<JobTask> pendingBack = new Queue<JobTask>();
        private readonly Queue<Action> pendingMain = new Queue<Action>();
        private readonly Queue<RoutineTask> pendingRoutine = new Queue<RoutineTask>();

        private volatile bool hasUpdates;
        private volatile bool hasBack;
        private volatile bool hasMain;
        private volatile bool hasRoutine;
        private static volatile bool alive;

        private DateTime lastUpdate;
#if USE_THREAD
        private Thread workThread;
        private Thread mainThread;

        /// <summary>
        /// Checks if this is the main thread
        /// </summary>
        public bool IsMainThread
        {
            get { return Thread.CurrentThread == mainThread; }
        }
        
#else
        /// <summary>
        /// Checks if this is the main thread
        /// </summary>
        public bool IsMainThread
        {
            get { return true; }
        }
#endif

        void Awake()
        {
#if USE_THREAD
            mainThread = Thread.CurrentThread;
#endif
            _instance = this;
        }

        void Start()
        {
#if USE_THREAD
            workThread = new Thread(() =>
            {
                while (alive)
                {
                    try
                    {
                        lock (pendingBack)
                        {
                            while (pendingBack.Count > 0)
                            {
                                var job = pendingBack.Dequeue();
                                job.Background();
                                if (job.Main != null)
                                    RunMainThread(job.Main);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.LogException("UnityThreadService", ex);
                    }

                    //60 fps
                    Thread.Sleep(16);
                }

            });
            workThread.IsBackground = true;
            workThread.Start();
#endif
            // lastUpdate = DateTime.UtcNow;
        }

        void OnDestroy()
        {
            _instance = null;
            alive = false;
        }

        void Update()
        {
            // var delta = (DateTime.UtcNow - lastUpdate).TotalMilliseconds;
            // lastUpdate = DateTime.UtcNow;

            if (hasUpdates)
            {
                lock (pendingUpdates)
                {
                    for (int i = 0; i < pendingUpdates.Count; i++)
                    {
                        pendingUpdates[i].Action(Time.deltaTime);
                    }

                }

            }

            if (hasMain)
            {
                try
                {
                    lock (pendingMain)
                    {
                        while (pendingMain.Count > 0)
                        {
                            pendingMain.Dequeue()();
                        }
                        hasMain = false;
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogException("UnityThreadService.pendingMain", ex);
                }
            }

            if (hasRoutine)
            {
                try
                {
                    lock (pendingRoutine)
                    {
                        while (pendingRoutine.Count > 0)
                        {
                            StartCoroutine(pendingRoutine.Dequeue().Routine);

                            hasRoutine = false;
                        }
                    }

                }
                catch (Exception ex)
                {
                    LogService.LogException("UnityThreadService.pendingRoutine", ex);
                }
            }
        }

        IEnumerator RunDelayAsync(Action callback, float seconds, RoutineTask task)
        {
            yield return new WaitForSecondsRealtime(seconds);

            if (!task.IsDisposed)
                callback();
        }

        void Remove(UpdateTask task)
        {
            lock (pendingUpdates)
            {
                pendingUpdates.Remove(task);
                hasUpdates = pendingUpdates.Count > 0;
            }
        }
        #endregion
    }
}