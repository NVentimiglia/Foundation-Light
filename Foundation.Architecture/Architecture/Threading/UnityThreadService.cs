using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
#if USE_THREAD
using System.Threading;
#endif

namespace Foundation.Architecture
{
    public class UnityThreadService : MonoBehaviour, IThreadingService
    {
        #region Defines
        public class UpdateTask : IDisposable
        {
            public Action<double> Action;

            public bool IsDisposed
            {
                get { return Action != null; }
            }

            public void Dispose()
            {
                Action = null;
                ThreadSafePool<UpdateTask>.Default.Return(this);
            }
        }

        public class RoutineTask : IDisposable
        {
            public IEnumerator Action;

            public bool IsDisposed
            {
                get { return Action != null; }
            }

            public void Dispose()
            {
                Action = null;
                ThreadSafePool<RoutineTask>.Default.Return(this);
            }
        }
        #endregion

        #region API
        public IDisposable RunUpdate(Action<double> callback)
        {
            var task = ThreadSafePool<UpdateTask>.Default.Rent();
            task.Action = callback;

            lock (pendingUpdates)
            {
                pendingUpdates.Add(task);
                hasUpdates = true;
            }

            return task;
        }

        public IDisposable RunDelay(Action callback, int intervalMs = 5000)
        {
            var task = ThreadSafePool<RoutineTask>.Default.Rent();
            task.Action = RunDelayAsync(callback, intervalMs, task);

            lock (pendingRoutine)
            {
                pendingRoutine.Enqueue(task);
                hasRoutine = true;
            }

            return task;
        }

        public void RunRoutine(IEnumerator routine)
        {
            var task = ThreadSafePool<RoutineTask>.Default.Rent();
            task.Action = routine;

            lock (pendingRoutine)
            {
                pendingRoutine.Enqueue(task);
                hasRoutine = true;
            }
        }

        public void RunMainThread(Action action)
        {
            lock (pendingMain)
            {
                pendingMain.Enqueue(action);
                hasMain = true;
            }
        }

        public void RunBackgroundThread(Action action)
        {
            lock (pendingBack)
            {
                pendingBack.Enqueue(action);
                hasBack = true;
            }
        }

        #endregion

        #region Implementation
        private static UnityThreadService _instance;
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
        private readonly Queue<Action> pendingBack = new Queue<Action>();
        private readonly Queue<Action> pendingMain = new Queue<Action>();
        private readonly Queue<RoutineTask> pendingRoutine = new Queue<RoutineTask>();

        private volatile bool hasUpdates;
        private volatile bool hasBack;
        private volatile bool hasMain;
        private volatile bool hasRoutine;

        private DateTime lastUpdate;
#if USE_THREAD
        private Thread workThread;
#endif

        void Start()
        {
#if USE_THREAD
            workThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        lock (pendingBack)
                        {
                            while (pendingBack.Count > 0)
                            {
                                pendingBack.Dequeue()();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException("UnityThreadService", ex);
                    }
                }
            });
            workThread.IsBackground = true;
            workThread.Start();
#endif
            lastUpdate = DateTime.UtcNow;
        }

        void Update()
        {
            var delta = DateTime.UtcNow - lastUpdate;
            lastUpdate = DateTime.UtcNow;

            if (hasUpdates)
            {
                lock (pendingUpdates)
                {
                    for (int i = 0; i < pendingUpdates.Count; i++)
                    {
                        pendingUpdates[i].Action(Time.deltaTime);
                    }
                    hasUpdates = false;
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
                    Logger.LogException("UnityThreadService.pendingMain", ex);
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
                            StartCoroutine(pendingRoutine.Dequeue().Action);

                            hasRoutine = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException("UnityThreadService.pendingRoutine", ex);
                }
            }
        }

        IEnumerator RunDelayAsync(Action callback, int intervalMs, RoutineTask task)
        {
            yield return new WaitForSecondsRealtime(intervalMs);

            if (!task.IsDisposed)
                callback();
        }
        #endregion
    }
}