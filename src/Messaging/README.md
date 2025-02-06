<center>

<img src="https://avatars.githubusercontent.com/u/172126989?s=400&u=930ba2bd7e78a6be9c4bd504d656f29453d74a80&v=4" alt="logo" style="width: 250px; margin-bottom: 8px;" />

---

NanoWorks creates small yet powerful libraries that add significant value to software projects. Our open-source libraries are licensed under Apache 2.0, allowing free use, modification, and distribution.

---

</center>

### NanoWorks.Messaging

NanoWorks.Messaging is a lightweight and flexible distributed messaging library designed to simplify the implementation of asynchronous messaging systems. It provides an easy-to-use API for building scalable applications with publisher-subscriber patterns.


The library currently supports [RabbitMQ](https://www.rabbitmq.com/) as the messaging broker, allowing seamless integration into your distributed architecture.

---

### Overview

NanoWorks.Messaging.RabbitMq features three main components:

1. **Publishers:** publish messages to one or many consumers.
2. **Consumers:** subscribe to queues bound to specific exchanges, processing incoming messages.
3. **Messages:** each message type is routed through an exchange using a fully qualified name, like "Sample.WebApi.Models.Events.AuthorUpdatedEvent".

Consumers' queues are bound to the exchanges for the message types they handle, ensuring proper message delivery.

---

### Important Considerations

- **Avoid Name Collisions:** use distinct RabbitMQ virtual hosts or servers to prevent collisions between exchange and queue names.
- **Manage Orphaned Exchanges and Queues:** if exchanges and queues are not set to auto-delete, orphaned items may accumulate, requiring manual cleanup. This can happen when queue names or message types change.

---

### Dead Letters

Messages that fail processing or exceed the retry limit are published to the `NanoWorks.DeadLetter` queue. By default, messages that fail serialization are also sent to this queue, but they can be configured to be ignored and discarded.

---

### Faults and Exceptions

- When a consumer throws an exception during message processing, a `TransportError` with details will be published.
- If the retry limit has not been exceeded, the message is sent to the consumer's retry queue for reprocessing.
- `TransportError` messages can be consumed by subscribing to the `NanoWorks.TransportError` type.
- To prevent error loops, ensure that `TransportError` endpoints handle exceptions using a fallback logger.

```
options.ConfigureMessageConsumer<SomeConsumer>(consumerOptions =>
{
    consumerOptions.Subscribe<TransportError>(consumer => consumer.OnTransportError);
});
```

```
public async Task OnTransportError(TransportError transportError, CancellationToken cancellationToken)
{
    try
    {
        // Do something with the transport error
    }
    catch (Exception error)
    {
        // Log the exception
    }
}
```

---

### Getting Started

1. Install the [NanoWorks.Messaging.RabbitMq](https://www.nuget.org/packages/NanoWorks.Messaging.RabbitMq) NuGet package.

2. Create a message consumer class with methods that handle specific types of messages.

    - A message endpoint must be a public method with two parameters.

    - The first parameter must be the message model and the second a `CancellationToken`.

```
public sealed class CacheConsumer(
    ICache<AuthorDto> authorCache,
    ICache<BookDto> bookCache)
{
    public async Task OnAuthorUpdated(AuthorUpdatedEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{nameof(Author)} updated '{@event.AuthorId}' - refreshing cache");
        await authorCache.RefreshAsync(@event.AuthorId.ToString(), cancellationToken);
    }

    public async Task OnBookUpdated(BookUpdatedEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{nameof(Book)} updated '{@event.BookId}' - refreshing cache");
        await authorCache.RefreshAsync(@event.AuthorId.ToString(), cancellationToken);
        await bookCache.RefreshAsync(@event.BookId.ToString(), cancellationToken);
    }
}
```

3. Add the NanoWorks.Messaging.RabbitMq dependencies to the `IServiceCollection` at start-up, and configure the options to meet your requirements.

```
builder.Services.AddNanoWorksRabbitMq(options =>
{
    options.UseConnectionString("[your connection string]");

    options.ConfigureMessagePublisher(publisherOptions =>
    {
        publisherOptions.OnSerializationException(PublisherSerializerExceptionBehavior.Ignore);
    });

    options.ConfigureMessageConsumer<CacheConsumer>(consumerOptions =>
    {
        consumerOptions.Name(nameof(CacheConsumer));
        consumerOptions.MaxMessageConcurrency(10);
        consumerOptions.MessageTtl(TimeSpan.FromHours(1));
        consumerOptions.Retries(maxRetryCount: 3, retryDelay: TimeSpan.FromSeconds(1));
        consumerOptions.AutoDelete();
        consumerOptions.OnSerializationException(ConsumerSerializerExceptionBehavior.DeadLetter);

        consumerOptions.Subscribe<AuthorUpdatedEvent>(consumer => consumer.OnAuthorUpdated);
        consumerOptions.Subscribe<BookUpdatedEvent>(consumer => consumer.OnBookUpdated);
    });
});
```

4. Publish messages

    - An instance of `IMessagePublisher` is available via the service provider / dependency injection.

```
var authorUpdatedEvent = new AuthorUpdatedEvent { AuthorId = author.AuthorId };

// broadcast message to all subscribed consumers
await messagePublisher.BroadcastAsync(authorUpdatedEvent);

// send message to a specific consumer
await messagePublisher.SendAsync(nameof(CacheConsumer), authorUpdatedEvent);
```