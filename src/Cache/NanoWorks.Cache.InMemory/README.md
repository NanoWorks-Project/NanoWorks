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

#### Download / install the [NanoWorks.Cache.InMemory](https://www.nuget.org/packages/NanoWorks.Cache.InMemory) NuGet package

#### Create a `InMemoryCacheContext`

```
/// <summary>
/// Test cache.
/// </summary>
public sealed class TestCache : InMemoryCacheContext
{
}
```

#### Define and configure `InMemoryCacheSet`

```
/// <summary>
/// Test cache.
/// </summary>
public sealed class TestCache : InMemoryCacheContext
{
    public TestCache()
    {
        TestSet = NewSet<CacheTestItem, Guid>(cacheSetOptions =>
        {
            cacheSetOptions.Table($"{nameof(CacheTestItem)}");
            cacheSetOptions.Key(item => item.Id);
            cacheSetOptions.Expiration(TimeSpan.FromSeconds(10));
        });
    }

    public InMemoryCacheSet<CacheTestItem, Guid> TestSet { get; set; }
}
```

### Add your `InMemoryCacheContext` to the service collection

```
var serviceProvider = new ServiceCollection()
    .AddNanoWorksInMemoryCache<TestCache>()
    .BuildServiceProvider();
```

### Use your `InMemoryCacheContext`

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