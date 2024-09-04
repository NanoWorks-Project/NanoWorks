// Ignore Spelling: Nano

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using NanoWorks.Cache.Abstractions;
using NanoWorks.Cache.Implementations;

namespace NanoWorks.Cache.Options;

/// <summary>
/// Options for caching.
/// </summary>
public class CacheOptions
{
    private static readonly HashSet<Type> _cacheTypes = [];
    private readonly IServiceCollection _services;

    internal CacheOptions(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    /// Adds a cache for items of type <typeparamref name="TItem"/> to the service collection.
    /// </summary>
    /// <typeparam name="TItem">Type of item in the cache.</typeparam>
    /// <param name="configure">Action to configure the cache options.</param>
    public void UseCache<TItem>(Action<ItemCacheOptions<TItem>> configure)
        where TItem : class, new()
    {
        if (_cacheTypes.Contains(typeof(TItem)))
        {
            throw new InvalidOperationException($"Cache for type {typeof(TItem).Name} already exist.");
        }

        var itemCacheOptions = new ItemCacheOptions<TItem>();
        configure(itemCacheOptions);
        itemCacheOptions.Validate();

        _services.AddSingleton(itemCacheOptions);
        _services.AddScoped(itemCacheOptions.CacheSourceType);

        _services.AddScoped<ICache<TItem>>(sp =>
        {
            var distributedCache = sp.GetRequiredService<IDistributedCache>();
            var options = sp.GetRequiredService<ItemCacheOptions<TItem>>();
            var itemCache = new ItemCache<TItem>(sp, distributedCache, options);
            return itemCache;
        });

        _cacheTypes.Add(typeof(TItem));
    }
}
