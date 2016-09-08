# Foundation

A Framework level library for client / server development. Built with MSN best practices in mind.

- **Features**
  - Super light weight, take and use what you need.
  - **Unit tests** for everything
  - Cross compiles to run in Unity3d, .net45, and DotNetCore.
  - Injector for dependency lookup. Supports transient and singleton.
  - MessageBroker for relaying messages globaly or by Object
  - Logging service for tracing out information and errors.
  - Pooling built-in, to lower GC pressure from messages.
  - Observable framework with highly cached proxy layer for MVVM support
  - Cross platform threading, timer, timeout, and coroutine helpers (TODO)

[Injector : Service Locator / Inversion of control module](Injector.md) 

[Domain Events : Global Message Broker](DomainEvents.md) 

[Object Events : Routed Message broker / SendMessage replacement](ObjectEvents.md) 

[Observables : INotifyPropertyChange and MVVM](Observable.md) 

[Logging : Cross platform debug proxy](Logging.md) 

[Threading : Cross platform timer, update, coroutines and task runniner](Threading.md) 
