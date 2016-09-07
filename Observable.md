# Observables

Observables is a design pattern where objects can listen to other objects for update notification. The most practical use case of this is MVVM, where a view will 'bind' to a view model and redraw itself when that view model changes.

## Key components 

**IPropertyChanged**
The standard 'microsoft way' of making an object observable. Includes a single event with the changed member's name.
 
 `````
    public delegate void PropertyChanged(string memberName);

    public interface IPropertyChanged
    {
        event PropertyChanged OnPropertyChanged;
    }
 `````
 
**ObservableObject**

Implements IPropertyChange for poco objects
 
**ObservableBehaviour**

Implements IPropertyChange for MonoBehaviours objects

**ObservableCollection**

A specialized list with events for when items are added, removed, or changed.

**Observable**

An alternative way of observing an object which does not use magic strings. This object is generic and includes the change event internally.

**ObservableProxy**

Reflection is heavy and costly. This class makes it better. As a bonus, acts as a proxy for binding to non MVVM objects.

 - Wraps observable's (all of the above) and exposes an easy to use interface that binders can listen to.
 - Heavily cached since reflection is expensive
 - Supports methods and properties
 - Supports Observable<> fields
 - Supports Non-IPropertyChanged and simple DTO's. (Those objects will be not raise change events, however)
 

## Metrics

here are my current stats for calling a method 1000 times. Time is in StopWatch ticks.
 
 - Normal 81
 - Reflected 1088
 - Cached 82
 - Proxy 182 (100 of that from dictionary lookup, 82 from the call)
            
