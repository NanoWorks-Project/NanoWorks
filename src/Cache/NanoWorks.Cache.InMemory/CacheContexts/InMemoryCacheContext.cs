// Ignore Spelling: Nano

using System;
using NanoWorks.Cache.InMemory.CacheSets;
using NanoWorks.Cache.InMemory.Options;

namespace NanoWorks.Cache.InMemory.CacheContexts;

/// <summary>
/// In memory cache context.
/// </summary>
public abstract class InMemoryCacheContext
{
    /// <summary>
    /// Creates a new cache set with the specified options.
    /// </summary>
    /// <typeparam name="TItem">Type of the item in the cache set.</typeparam>
    /// <typeparam name="TKey">Type of the key for the cache set.</typeparam>
    /// <param name="configure">Action to configure the cache set options.</param>
    public InMemoryCacheSet<TItem, TKey> NewSet<TItem, TKey>(Action<InMemoryCashSetOptions<TItem, TKey>> configure)
        where TItem : class, new()
    {
        var options = new InMemoryCashSetOptions<TItem, TKey>();
        configure(options);
        options.Validate();
        return new InMemoryCacheSet<TItem, TKey>(options);
    }
}
