using System;
using System.Collections;
using System.Threading.Tasks;

namespace Foundation.Architecture
{
    public class AsyncThreadService : IThreadingService
    {
        public struct ThreadTask : IDisposable
        {
            public bool IsDisposed;

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        public IDisposable RunUpdate(Action<double> callback)
        {
            var task = new ThreadTask();
            RunUpdate(callback, task);
            return task;
        }

        async void RunUpdate(Action<double> callback, ThreadTask task)
        {
            var startTime = DateTime.UtcNow;
            double delta = 0;

            while (!task.IsDisposed)
            {
                //60 fps
                await Task.Delay(16);
                delta = (DateTime.UtcNow - startTime).TotalMilliseconds;
                startTime = DateTime.UtcNow;
                callback(delta);
            }
        }

        public IDisposable RunDelay(Action callback, int intervalMs = 5000)
        {
            var task = new ThreadTask();
            RunUpdate(callback, intervalMs, task);
            return task;
        }
        
        async void RunUpdate(Action callback, int intervalMs, ThreadTask task)
        {
            await Task.Delay(intervalMs);

            if (!task.IsDisposed)
            {
                callback();
            }
        }

        public IDisposable RunDelay<TState>(Action<TState> callback, TState state, int intervalMs = 5000)
        {
            var task = new ThreadTask();
            RunUpdate(callback, state, intervalMs, task);
            return task;
        }

        async void RunUpdate<TState>(Action<TState> callback, TState state, int intervalMs, ThreadTask task)
        {
            await Task.Delay(intervalMs);

            if (!task.IsDisposed)
            {
                callback(state);
            }
        }

        public IDisposable RunRoutine(IEnumerator routine)
        {
            var task = new ThreadTask();
            RunRoutine(routine, task);
            return task;
        }

        async void RunRoutine(IEnumerator routine, ThreadTask task)
        {
            do
            {
                if (task.IsDisposed)
                    return;

                //60 fps
                await Task.Delay(16);

            } while (routine.MoveNext());
        }
        
        public IDisposable RunRoutine(Func<IEnumerator> routine)
        {
            var task = new ThreadTask();
            RunRoutine(routine, task);
            return task;
        }

        public IDisposable RunRoutine<TState>(Func<TState, IEnumerator> routine, TState state)
        {
            var task = new ThreadTask();
            RunRoutine(routine, state, task);
            return task;
        }

        public async void RunRoutine<TState>(Func<TState, IEnumerator> func, TState state, ThreadTask task)
        {
            var routine = func(state);

            do
            {
                if (task.IsDisposed)
                    return;

                //60 fps
                await Task.Delay(16);

            } while (routine.MoveNext());
        }

        async void RunRoutine(Func<IEnumerator> func, ThreadTask task)
        {
            var routine = func();

            do
            {
                if (task.IsDisposed)
                    return;

                //60 fps
                await Task.Delay(16);

            } while (routine.MoveNext());
        }

        public void RunMainThread(Action action)
        {
            //NotImplemented
            action();
        }

        public void RunMainThread<TState>(Action<TState> action, TState state)
        {
            //NotImplemented
            action(state);
        }

        public void RunBackgroundThread(Action action)
        {
            //NotImplemented
            action();
        }

        public void RunBackgroundThread<TState>(Action<TState> action, TState state)
        {
            //NotImplemented
            action(state);
        }
    }
}