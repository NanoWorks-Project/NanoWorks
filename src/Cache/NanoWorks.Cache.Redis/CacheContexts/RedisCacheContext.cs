// Ignore Spelling: Nano

using System;
using NanoWorks.Cache.Redis.CacheSets;
using NanoWorks.Cache.Redis.Options;
using StackExchange.Redis;

namespace NanoWorks.Cache.Redis.CacheContexts;

/// <summary>
/// Redis cache context.
/// </summary>
public abstract class RedisCacheContext
{
    private readonly RedisCacheContextOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisCacheContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">Options for the cache context.</param>
    public RedisCacheContext(RedisCacheContextOptions options)
    {
        _options = options;
    }

    private IConnectionMultiplexer Connection => _options.Connection;

    /// <summary>
    /// Creates a new cache set with the specified options.
    /// </summary>
    /// <typeparam name="TItem">Type of the item in the cache set.</typeparam>
    /// <typeparam name="TKey">Type of the key for the cache set.</typeparam>
    /// <param name="configure">Action to configure the cache set options.</param>
    public RedisCacheSet<TItem, TKey> NewSet<TItem, TKey>(Action<RedisCashSetOptions<TItem, TKey>> configure)
        where TItem : class, new()
    {
        var options = new RedisCashSetOptions<TItem, TKey>();
        configure(options);
        options.Validate();
        return new RedisCacheSet<TItem, TKey>(Connection, options);
    }
}
