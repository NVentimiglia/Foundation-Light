# Observables

Observables is a design pattern where objects can listen to other objects for update notification. The most practical use case of this is MVVM, where a view will 'bind' to a view model and redraw itself when that view model changes.


## Key components 

**IObservable**
A event broadcaster. Something that can be listened to.
 
 `````
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
 `````
 
**MVVMObservable**

 `````
     
    /// <summary>
    /// Raises PropertyChange events for the view to listen to.
    /// </summary>
    public interface IMvvmObservable : IObservable<PropertyEvent> {}
 
    /// <summary>
    /// MVVM Event Arguments
    /// </summary>
    public struct PropertyEvent
    {
        /// <summary>
        /// The Model
        /// </summary>
        public object Sender;

        /// <summary>
        /// Member name (property / method)
        /// </summary>
        public string Name;

        /// <summary>
        /// New Value
        /// </summary>
        public object Value;

        /// <summary>
        /// Refresh root
        /// </summary>
        public bool IsRefreshAll
        {
            get { return string.IsNullOrEmpty(Name); }
        }
    }
 `````
 
**ObservableObject**

Implements IMvvmObservable for poco objects
 
**ObservableBehaviour**

Implements IMvvmObservable for MonoBehaviours objects

**ObservableCollection**

A specialized list with events for when items are added, removed, or changed.

**ObservableProperty**

An alternative way of observing an object which does not use magic strings. This object is generic and includes the change event internally.

**ObservableProxy**

Reflection is heavy and costly. This class makes it better. As a bonus, acts as a proxy for binding to non MVVM objects.

 - Wraps observable's (all of the above) and exposes an easy to use interface that binders can listen to.
 - Heavily cached since reflection is expensive
 - Supports methods and properties
 - Supports Non-IPropertyChanged and simple DTO's. (Those objects will be not raise change events, however)
 

## Metrics

here are my current stats for calling a method 1000 times. Time is in StopWatch ticks.
 
 - Normal 81
 - Reflected 1088
 - Cached 82
 - Proxy 182 ( 2 method calls + dictionary lookup. one to proxy, one to instance)

