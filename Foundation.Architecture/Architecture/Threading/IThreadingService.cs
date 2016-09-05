using System;

namespace Foundation.Architecture
{

    /// <summary>
    /// Main thread / Background thread utility
    /// </summary>
    public interface IThreadingService
    {
        void OnMainThread(Action action);

        void OnMainThread(Action<object> action, object state);

        void OnMainThread<T>(Action<T> action, T state);

        //

        void OnBackgroundThread(Action action);

        void OnBackgroundThread(Action<object> action, object state);

        void OnBackgroundThread<T>(Action<T> action, T state);
    }
}
