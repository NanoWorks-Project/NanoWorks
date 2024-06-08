<center>

<img src="https://avatars.githubusercontent.com/u/172126989?s=400&u=930ba2bd7e78a6be9c4bd504d656f29453d74a80&v=4" alt="logo" style="width: 250px; margin-bottom: 8px;" />

---

NanoWorks creates **_small_** libraries that provide **_big_** value to software projects. 

The libraries are open-source and offered under the Apache 2.0 license.

---

</center>

### Cache

NanoWorks.Cache is inspired by Entity Framework's [DbSet.](https://learn.microsoft.com/en-us/dotnet/api/system.data.entity.dbset-1?view=entity-framework-6.2.0) This library offers a similar syntax and coding experience for caching. 

NanoWorks.Cache currently supports [Redis.](https://redis.io/)

---

### Getting Started

#### Download / install the <i>NanoWorks.Cache</i> NuGet package

#### Create a <i>RedisCacheContext</i>

```
public class BookStoreCache : RedisCacheContext
{
    public BookStoreCache(CacheContextOptions options) : base(options)
    {
    }
}
```

#### Define and configure <i>CacheSets</i>

```
public class BookStoreCache : RedisCacheContext
{
    public BookStoreCache(CacheContextOptions options) : base(options)
    {
        Authors = NewSet<Author, Guid>(options =>
        {
            options.Table("Authors");
            options.Key(author => author.AuthorId);
            options.Expiration(TimeSpan.FromHours(1));
            options.OnSerializationException(SerializerExceptionBehavior.Ignore);
        });

        AuthorBooks = NewSet<AuthorBook, Guid>(options =>
        {
            options.Table("AuthorBooks");
            options.Key(authorBook => authorBook.AuthorId);
            options.Expiration(TimeSpan.FromHours(1));
            options.OnSerializationException(SerializerExceptionBehavior.Ignore);
        });

        Books = NewSet<Book, Guid>(options =>
        {
            options.Table("Books");
            options.Key(book => book.BookId);
            options.Expiration(TimeSpan.FromHours(1));
            options.OnSerializationException(SerializerExceptionBehavior.Ignore);
        });
    }

    public CacheSet<Author, Guid> Authors { get; }

    public CacheSet<AuthorBook, Guid> AuthorBooks { get; }

    public CacheSet<Book, Guid> Books { get; }
}
```

### Add your <i>RedisCacheContext</i> to the service collection

```
builder.Services.AddNanoWorksRedisCache<BookStoreCache>(options =>
{
    options.UseConnectionString("localhost:6379");
    options.UseConnectionPoolSize(Environment.ProcessorCount);
});

builder.Services.AddScoped<IBookStoreCache, BookStoreCache>();
```

### Use your <i>RedisCacheContext</i>

```
[ApiController, Route("api")]
public class BookStoreController(IBookStoreCache cache) : ControllerBase
{
    [HttpGet("authors")]
    public IActionResult GetAuthors()
    {
        var authors = cache.Authors;
        return Ok(authors);
    }
}
```

---

### Cache Context Options

Connection string
```
public void UseConnectionString(string connectionString)
```
Connection pool size
```
public void UseConnectionPoolSize(int size)
```
---

### Cache Set Options

Table name for the cache set
```
public CashSetOptions<TItem, TKey> Table(string tableName)
```
Key selector - function to get the key for an item in the cache set
```
public CashSetOptions<TItem, TKey> Key(Func<TItem, TKey> keySelector)
```
Cache set expiration - automatically resets when items are create, updated, or read from the cache set
```
public CashSetOptions<TItem, TKey> Expiration(TimeSpan expirationDuration)
```
Serialization exception behavior - behavior when the value in the cache fails to be serialized
```
public CashSetOptions<TItem, TKey> OnSerializationException(SerializerExceptionBehavior behavior)
```
```
public enum SerializerExceptionBehavior
{
    Ignore,
    Throw
}
```

---

### Best Practices

Avoid non-indexed queries against cache sets with many items.

To optimize indexed queries against the cache, use models to track related items with <i>HashSet</i>.

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

Run the docker compose file to start Redis
```
[root]\src\Cache\Sample\docker\docker-compose
```

Build the project, and run it
```
[root]\src\Cache\Sample\Sample.WebApi\Sample.WebApi.csproj
```