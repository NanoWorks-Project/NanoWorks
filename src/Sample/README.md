<center>

<img src="https://avatars.githubusercontent.com/u/172126989?s=400&u=930ba2bd7e78a6be9c4bd504d656f29453d74a80&v=4" alt="logo" style="width: 250px; margin-bottom: 8px;" />

---

NanoWorks creates small yet powerful libraries that add significant value to software projects. Our open-source libraries are licensed under Apache 2.0, allowing free use, modification, and distribution.

---

</center>

### Sample App

This sample web API for a bookstore includes endpoints for interacting with Author and Book resources. It integrates a data layer using Entity Framework for database operations and NanoWorks.Cache for caching with [Redis](https://redis.io/).

Key features:

- **Entity Framework:** manages database context for Author and Book resources.
- **NanoWorks.Cache:** provides a caching layer for improved performance.
- **NanoWorks.Messaging.RabbitMq:** publishes and consumes events via RabbitMQ to asynchronously refresh the cache when resources are created or modified.

---

### Getting Started

1. Run the `docker-compose` file to start Redis, PostgreSQL, and RabbitMQ
```
[root]\src\Sample\docker\docker-compose
```

2. Build and run the project
```
[root]\src\Sample\Sample.WebApi\Sample.WebApi.csproj
```