<center>

<img src="https://avatars.githubusercontent.com/u/172126989?s=400&u=930ba2bd7e78a6be9c4bd504d656f29453d74a80&v=4" alt="logo" style="width: 250px; margin-bottom: 8px;" />

---

NanoWorks creates small yet powerful libraries that add significant value to software projects. Our open-source libraries are licensed under Apache 2.0, allowing free use, modification, and distribution.

</center>

---

### NanoWorks.Cache

Built on Microsoft's [IDistributedCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache?view=net-8.0), NanoWorks.Cache provides a type-safe and efficient solution for caching data in distributed environments.

---

### Overview

NanoWorks.Cache exposes an `ICache<TItem>` interface that provides methods to get, set, and remove items from the cache. Each `ICache<TItem>` instance is bound to an `ICacheSource<TItem>`, which automatically populates the cache with items behind the scenes. `ICacheSource<TItem>` is only used when an item is not found in the cache, or an item is refreshed at runtime by calling `ICache<TItem>.Refresh(string key)`.

---

### Getting Started

1. Install the [NanoWorks.Cache](https://www.nuget.org/packages/NanoWorks.Cache/) NuGet package.

2. Implement an `ICacheSource<TItem>` to populate the cache.

```
public class TestCacheSource(TestDbContext dbContext) : ICacheSource<AuthorSummary>
{
    public AuthorSummary? Get(string key)
    {
        var author = dbContext.Authors
            .Include(a => a.Books)
            .ThenInclude(a => a.Genre)
            .SingleOrDefault(a => a.Id == Guid.Parse(key));

        if (author is null)
        {
            return null;
        }

        var authorSummary = new AuthorSummary(author);
        return authorSummary;
    }

    public async Task<AuthorSummary?> GetAsync(string key, CancellationToken cancellationToken)
    {
        var author = await dbContext.Authors
            .Include(a => a.Books)
            .ThenInclude(a => a.Genre)
            .SingleOrDefaultAsync(a => a.Id == Guid.Parse(key), cancellationToken);

        if (author is null)
        {
            return null;
        }

        var authorSummary = new AuthorSummary(author);
        return authorSummary;
    }
}
```

3. Add an `IDistributedCache` implementation to the service collection.

```
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});
```

4. Configure the cache.

```
services.AddNanoWorksCaching(options =>
{
    options.UseCache<AuthorSummary>(cacheOptions =>
    {
        cacheOptions.ExpirationDuration = TimeSpan.FromSeconds(10);
        cacheOptions.Key(authorSummary => authorSummary.AuthorId.ToString());
        cacheOptions.Source<TestCacheSource>();
    });
});
```

5. Inject `ICache<TItem>` and use it.

```
public class AuthorService
{
    private readonly ICache<AuthorSummary> _cache;

    public AuthorService(ICache<AuthorSummary> cache)
    {
	    _cache = cache;
    }

    public async Task<AuthorSummary?> GetAuthorSummary(Guid authorId)
    {
	    return await _cache.GetAsync(authorId.ToString());
    }
}
```
