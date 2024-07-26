// Ignore Spelling: Nano

using NanoWorks.Cache.InMemory.CacheContexts;
using NanoWorks.Cache.InMemory.CacheSets;

namespace NanoWorks.Cache.InMemory.Tests.TestObjects;

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
