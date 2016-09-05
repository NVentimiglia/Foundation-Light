# Foundation

A Framework level library for client / server development. Built with MSN best practices in mind.

**Work in progress**

- **Features**
  - Super light weight, take and use what you need.
  - Cross compiles to run in Unity3d and DotNetCore, keeping logic.
  - Injector for dependency lookup. Supports transient and singleton.
  - Generic MessageBroker for relaying global or routed events internal to the app.
  - Logging service for tracing out information and errors.
  - Pooling built-in, to lower GC pressure from messages.
  - Unified threading api with support for update loops, timers, and timeouts.
  - Includes MVVM support with databinding, view factory, and a navigational services.
  
  
- **External Dependencies**
  - Protobuff (client and server) and Json.Net (server only)

- **Future Plans**
  - Finnish Threading feature
  - Add UnityTasks (Response wrapper with CustomYieldInstructions)
  - RestAPIClient with Bearer Authentication
  - Tests  
  - Example App
  - Networking Package with support for UDP, WebSockets, and MVC style routing.