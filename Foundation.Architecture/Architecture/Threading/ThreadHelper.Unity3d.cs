using System;
using System.Collections.Generic;
using System.Threading;

namespace SignalMQ.Infrastructure.Tools
{
    /// <summary>
    /// Main thread utility
    /// </summary>
    public class ThreadHelper
    {
        public struct ActionState
        {
            public Action<object> Callback;
            public object Param;
        }
        
        static Queue<Action> _pendingAction = new Queue<Action>();
        static Queue<ActionState> _pendingState = new Queue<ActionState>();
        static UpdateProxy _update;
        static bool pending;
        static object _lock = new object();

        static ThreadHelper()
        {
            _update = UpdateProxy.Get(Dispatch);
        }

        /// <summary>
        /// Must be called once on main thread
        /// </summary>
        internal static void Init(){}

        /// <summary>
        /// Queues the action for dispatch on the UI thread
        /// </summary>
        public static void OnMainThread(Action action)
        {
           // StackTraceHelper.Print();
            lock (_lock)
            {
                _pendingAction.Enqueue(action);
                pending = true;
            }
        }

        /// <summary>
        /// Queues the action for dispatch on the UI thread
        /// </summary>
        public static void OnMainThread(Action<object> action, object state)
        {
           // StackTraceHelper.Print();
            lock (_lock)
            {
                _pendingState.Enqueue(new ActionState
                {
                    Callback = action,
                    Param = state,
                });
                pending = true;
            }
        }

        /// <summary>
        /// Queues the action for dispatch on the UI thread
        /// </summary>
        public static void OnMainThread<T>(Action<T> action, T state)
        {
          //  StackTraceHelper.Print();
            lock (_lock)
            {
                _pendingState.Enqueue(new ActionState
                {
                    Callback = o => action((T)o),
                    Param = state,
                });
                pending = true;
            }
        }

        static void Dispatch()
        {
            if(!pending)
                return;

            lock (_lock)
            {
                while (_pendingAction.Count > 0)
                {
                    _pendingAction.Dequeue()();
                }

                while (_pendingState.Count > 0)
                {
                    var state = _pendingState.Dequeue();
                    state.Callback.DynamicInvoke(state.Param);
                }

                pending = false;
            }
        }



        public static void OnBackgroundThread(Action action)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                action();
            });
        }

        public static void OnBackgroundThread(Action<object> action, object state)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                action(state);
            }, state);
        }

        public static void OnBackgroundThread<T>(Action<T> action, T state)
        {
            ThreadPool.QueueUserWorkItem(func =>
            {
                action((T)state);
            }, state);
        }
    }
}
