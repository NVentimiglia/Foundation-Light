// Nicholas Ventimiglia 2016-09-05

using System;

namespace Foundation.Architecture
{
    /// <summary>
    /// Data Source. Message Publisher Source
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IObservable<TModel> : IDisposable
    {
        /// <summary>
        /// Old School Change Delegate
        /// </summary>
        event Action<TModel> OnPublish; 
        
        /// <summary>
        /// Will Publish value
        /// </summary>
        /// <param name="model"></param>
        void Publish(TModel model);
    }
}