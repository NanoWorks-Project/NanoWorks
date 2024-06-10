<center>

<img src="https://avatars.githubusercontent.com/u/172126989?s=400&u=930ba2bd7e78a6be9c4bd504d656f29453d74a80&v=4" alt="logo" style="width: 250px; margin-bottom: 8px;" />

---

NanoWorks creates **_small_** libraries that provide **_big_** value to software projects. 

The libraries are open-source and offered under the Apache 2.0 license.

---

</center>

### Sample App

This simple web API for a bookstore exposes endpoints to interact with Author and Book resources. 

It features a data layer with an Entity Framework database context and NanoWorks Redis cache context.

When resources are created or updated, events are published and consumed using the NanoWorks messaging library for RabbitMQ to asynchronously update items in the cache.

---

### Getting Started

Run the `docker-compose` file to start Redis, PostgreSQL, and RabbitMQ
```
[root]\src\Sample\docker\docker-compose
```

Build and run the project
```
[root]\src\Sample\Sample.WebApi\Sample.WebApi.csproj
```