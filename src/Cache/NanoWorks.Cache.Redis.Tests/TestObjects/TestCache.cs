// Ignore Spelling: Nano

using NanoWorks.Cache.Options;
using NanoWorks.Cache.Redis.CacheContexts;
using NanoWorks.Cache.Redis.CacheSets;
using NanoWorks.Cache.Redis.Options;

namespace NanoWorks.Cache.Redis.Tests.TestObjects;

/// <summary>
/// Test cache.
/// </summary>
internal class TestCache : RedisCacheContext
{
    public TestCache(CacheContextOptions<TestCache> options) : base(options)
    {
        TestSet = NewSet<CacheTestItem, Guid>(cacheSetOptions =>
        {
            cacheSetOptions.Table($"{nameof(CacheTestItem)}-{Guid.NewGuid()}");
            cacheSetOptions.Key(item => item.Id);
            cacheSetOptions.Expiration(TimeSpan.FromSeconds(5));
            cacheSetOptions.OnSerializationException(SerializerExceptionBehavior.Throw);
        });
    }

    public CacheSet<CacheTestItem, Guid> TestSet { get; set; }
}
