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
  - Unified threading api with support for update loops, timers, and timeouts.
  - Optomized reflection helpers and caching.
  - Includes MVVM support with databinding, view factory, and a navigational services.


## Injector
[Injector : Service Locator / Inversion of control module](Injector.md) 