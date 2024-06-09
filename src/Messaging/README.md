<center>

<img src="https://avatars.githubusercontent.com/u/172126989?s=400&u=930ba2bd7e78a6be9c4bd504d656f29453d74a80&v=4" alt="logo" style="width: 250px; margin-bottom: 8px;" />

---

NanoWorks creates **_small_** libraries that provide **_big_** value to software projects. 

The libraries are open-source and offered under the Apache 2.0 license.

---

</center>

### Messaging

`NanoWorks.Messaging` is a distributed messaging library that enables developers to build asynchronous messaging systems with publishers and subscribers.

`NanoWorks.Messaging` currently supports [RabbitMQ.](https://www.rabbitmq.com/)

---

### Overview

`NanoWorks.Messaging.RabbitMq` has three main components - publishers, consumers, and messages.

Publishers publish messages, and consumers subscribe to and consume messages.

Within RabbitMQ, each message type is assigned an exchange using the fully-qualified name ("Sample.WebApi.Models.Events.AuthorUpdatedEvent").

Each consumer is assigned a queue, and the queue is bound to all exchanges / message types it is subscribed to.

---

### Important Considerations

Avoid exchange and queue naming collisions by using different RabbitMQ virtual hosts and servers.  

If exchanges and queues are not configured to be auto-deleted, orphaned exchanges and queues are possible. This may require manual cleanup, and it can occur when configured queue names change, message types change, etc...

---

### Dead Letters

Any messages that fail to be processed or exceed the configured retry limit will be published to the `NanoWorks.DeadLetter` queue.

Messages that fail serialization can be ignored and discarded through configuration - by default they are sent to the `NanoWorks.DeadLetter` queue.

---

### Faults and Exceptions

If a consumer throws an exception while processing a message, a `TransportError` with the exception details will be published to the `NanoWorks.Errors` queue.

If the message retry limit has not been exceeded, the message will be re-published to the queue and processed again. 

---

### Getting Started

#### Download / install the [NanoWorks.Messaging.RabbitMq](https://www.nuget.org/packages/NanoWorks.Messaging.RabbitMq) NuGet package

#### Create a message consumer

Message endpoints must be a public method with two parameters inside the consumer.

The first parameter must be the message model and the second a `CancellationToken`.

```
public class CacheConsumer(IBookStoreDatabase bookStoreDatabase, IBookStoreCache bookStoreCache)
{
    public async Task OnAuthorUpdated(AuthorUpdatedEvent @event, CancellationToken token)
    {
        Console.WriteLine($"{nameof(Author)} updated '{@event.AuthorId}' - syncing with cache");

        var author = await bookStoreDatabase.Authors
            .SingleOrDefaultAsync(x => x.AuthorId == @event.AuthorId);

        if (author == null)
        {
            return;
        }

        var authorDto = new AuthorDto(author);
        bookStoreCache.Authors[authorDto.AuthorId] = authorDto;
    }

    public async Task OnBookUpdated(BookUpdatedEvent @event, CancellationToken token)
    {
        Console.WriteLine($"{nameof(Book)} updated '{@event.BookId}' - syncing with cache");

        var book = await bookStoreDatabase.Books
            .SingleOrDefaultAsync(x => x.BookId == @event.BookId);

        if (book == null)
        {
            return;
        }

        var bookDto = new BookDto(book);
        bookStoreCache.Books[bookDto.BookId] = bookDto;

        var authorBooks = bookStoreCache.AuthorBooks[bookDto.AuthorId] ?? new AuthorBooksDto(bookDto.AuthorId);
        authorBooks.BookIds.Add(bookDto.BookId);
        bookStoreCache.AuthorBooks[bookDto.AuthorId] = authorBooks;
    }
}
```

#### Add dependencies at startup

```
builder.Services.AddNanoWorksRabbitMq(options =>
{
    options.UseConnectionString("your connection string");

    options.AddMessagePublisher(publisherOptions =>
    {
        publisherOptions.OnSerializationException(PublisherSerializerExceptionBehavior.Ignore);
    });

    options.AddMessageConsumer<CacheConsumer>(consumerOptions =>
    {
        consumerOptions.Queue(nameof(CacheConsumer));
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

#### Publish messages

An instance of `IMessagePublisher` is available via the service provider / dependency injection.

```
var authorUpdatedEvent = new AuthorUpdatedEvent { AuthorId = author.AuthorId };
await messagePublisher.PublishAsync(authorUpdatedEvent);
```
___

### Sample App

Location

```
[root]\src\Cache\Sample
```

Run the docker compose file to start Redis, PostgreSQL, and RabbitMQ
```
[root]\src\Sample\docker\docker-compose
```

Build and run the project
```
[root]\src\Sample\Sample.WebApi\Sample.WebApi.csproj
```