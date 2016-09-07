# Foundation

A Framework level library for client / server development. Built with MSN best practices in mind.

**Work in progress**

- **Features**
  - Super light weight, take and use what you need.
  - Cross compiles to run in Unity3d and DotNetCore.
  - Injector for dependency lookup. Supports transient and singleton.
  - Generic MessageBroker for relaying global or routed events internal to the app.
  - Logging service for tracing out information and errors.
  - Pooling built-in, to lower GC pressure from messages.


[Injector : Service Locator / Inversion of control module](Injector.md) 

[Domain Events : Global Message Broker](DomainEvents.md) 

[Object Events : Routed Message broker / SendMessage replacement](ObjectEvents.md) 
