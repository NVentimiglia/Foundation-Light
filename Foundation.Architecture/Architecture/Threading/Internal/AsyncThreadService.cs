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
                ThreadSafePool<ThreadTask>.Default.Return(this);
            }
        }

        public IDisposable RunUpdate(Action<double> callback)
        {
            var task = ThreadSafePool<ThreadTask>.Default.Rent();
            task.IsDisposed = false;
            RunUpdate(callback, task);
            return task;
        }
        
        public IDisposable RunDelay(Action callback, int intervalMs = 5000)
        {
            var task = ThreadSafePool<ThreadTask>.Default.Rent();
            task.IsDisposed = false;
            RunUpdate(callback, intervalMs, task);
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

        async void RunUpdate(Action callback, int intervalMs, ThreadTask task)
        {
            await Task.Delay(intervalMs);

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