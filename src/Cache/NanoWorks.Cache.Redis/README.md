<center>

<img src="https://avatars.githubusercontent.com/u/172126989?s=400&u=930ba2bd7e78a6be9c4bd504d656f29453d74a80&v=4" alt="logo" style="width: 250px; margin-bottom: 8px;" />

---

NanoWorks creates **_small_** libraries that provide **_big_** value to software projects. 

The libraries are open-source and offered under the Apache 2.0 license.

---

</center>

### NanoWorks.Cache

`NanoWorks.Cache` is inspired by Entity Framework's [DbSet.](https://learn.microsoft.com/en-us/dotnet/api/system.data.entity.dbset-1?view=entity-framework-6.2.0) This library offers a similar syntax and coding experience for caching.

---

### Getting Started

#### Download / install the [NanoWorks.Cache.Redis](https://www.nuget.org/packages/NanoWorks.Cache.Redis) NuGet package

#### Create a `RedisCacheContext`

```
public class BookStoreCache : RedisCacheContext
{
    public BookStoreCache(CacheContextOptions options) : base(options)
    {
    }
}
```

#### Define and configure `RedisCacheSet`

```
/// <summary>
/// Cache for the book store.
/// </summary>
public sealed class BookStoreCache : RedisCacheContext, IBookStoreCache
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BookStoreCache"/> class.
    /// </summary>
    /// <param name="options"><see cref="RedisCacheContextOptions"/>.</param>
    public BookStoreCache(RedisCacheContextOptions options)
        : base(options)
    {
        Authors = NewSet<AuthorDto, Guid>(options =>
        {
            options.Table("Authors");
            options.Key(author => author.AuthorId);
            options.Expiration(TimeSpan.FromHours(1));
        });

        AuthorBooks = NewSet<AuthorBooksDto, Guid>(options =>
        {
            options.Table("AuthorBooks");
            options.Key(authorBook => authorBook.AuthorId);
            options.Expiration(TimeSpan.FromHours(1));
        });

        Books = NewSet<BookDto, Guid>(options =>
        {
            options.Table("Books");
            options.Key(book => book.BookId);
            options.Expiration(TimeSpan.FromHours(1));
        });
    }

    /// <inheritdoc />
    public RedisCacheSet<AuthorDto, Guid> Authors { get; }

    /// <inheritdoc />
    public RedisCacheSet<AuthorBooksDto, Guid> AuthorBooks { get; }

    /// <inheritdoc />
    public RedisCacheSet<BookDto, Guid> Books { get; }
}
```

### Add your `RedisCacheContext` to the service collection

```
builder.Services.AddNanoWorksRedisCache<BookStoreCache>(options =>
{
    options.UseConnectionString("your connection string");
    options.UseConnectionPoolSize(Environment.ProcessorCount);
});

builder.Services.AddScoped<IBookStoreCache, BookStoreCache>();
```

### Use your `RedisCacheContext`

```
[HttpGet("authors/{authorId}")]
public IActionResult GetAuthor(Guid authorId)
{
    var author = bookStoreCache.Authors[authorId];
    return Ok(author);
}
```

---

### Best Practices

Keep stale data out of the cache by using short expiration timespans.

Avoid non-indexed queries against cache sets with many items.

To optimize indexed queries against the cache, use models to track related items with `HashSet`.

```
public class AuthorBooks
{
    public Guid AuthorId { get; set; }
    public HashSet<Guid> BookIds { get; set; } = new HashSet<Guid>();
}
```

```
[HttpGet("authors/{authorId}/books")]
public IActionResult GetAuthorBooks(Guid authorId)
{
    var authorBooks = cache.AuthorBooks[authorId];

    if (authorBooks?.BookIds == null)
    {
        return Ok(Enumerable.Empty<Book>());
    }

    var books = cache.Books[authorBooks.BookIds];
    return Ok(books);
}
```

---

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