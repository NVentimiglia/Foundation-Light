using System;
using System.Collections;
using System.Threading.Tasks;

namespace Foundation.Architecture.Internal
{
    public class AsyncThreadService : IThreadingService
    {
        public class ThreadTask : IDisposable
        {
            public bool IsDisposed;

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
        
        public bool IsMainThread
        {
            get { return true; }
        }

        public IDisposable RunUpdate(Action<double> callback)
        {
            var task = new ThreadTask();
            task.IsDisposed = false;
            RunUpdate(callback, task);
            return task;
        }

        public IDisposable RunDelay(Action callback, float seconds = 5)
        {
            var task = new ThreadTask();
            task.IsDisposed = false;
            RunUpdate(callback, seconds, task);
            return task;
        }

        public void RunRoutine(IEnumerator routine)
        {
            RunRoutineAsync(routine);
        }

        public void RunMainThread(Action action)
        {
            //NotImplemented
            action();
        }

        public void RunBackgroundThread(Action action)
        {
            //NotImplemented
            action();
        }

        public void RunBackgroundThread(Action backgroundWork, Action mainWork)
        {
            //NotImplemented
            backgroundWork();
            mainWork();
        }

        async void RunUpdate(Action<double> callback, ThreadTask task)
        {
            var startTime = DateTime.UtcNow;
            double delta;

            while (!task.IsDisposed)
            {
                //60 fps
                delta = (DateTime.UtcNow - startTime).TotalMilliseconds;
                startTime = DateTime.UtcNow;
                callback(delta);
                await Task.Delay(16);
            }
        }

        async void RunUpdate(Action callback, float seconds, ThreadTask task)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));

            if (!task.IsDisposed)
            {
                callback();
            }
        }

        async void RunRoutineAsync(IEnumerator routine)
        {
            do
            {
                //60 fps
                await Task.Delay(16);

            } while (routine.MoveNext());
        }
    }
}