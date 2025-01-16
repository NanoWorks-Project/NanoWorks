// Ignore Spelling: Nano

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using NanoWorks.Cache.Abstractions;
using NanoWorks.Cache.Options;

namespace NanoWorks.Cache.Implementations;

internal class ItemCache<TItem>(IServiceProvider serviceProvider, IDistributedCache cache, ItemCacheOptions<TItem> options) : ICache<TItem>
    where TItem : class, new()
{
    private readonly string _prefix = typeof(TItem).Name;

    /// <inheritdoc />
    public TItem? this[string key]
    {
        get => Get(key);
        set => Set(value!);
    }

    /// <inheritdoc />
    public IEnumerable<TItem> this[IEnumerable<string> keys]
    {
        get => Get(keys);
    }

    /// <inheritdoc />
    public TItem? Get(string key)
    {
        var itemJson = cache.GetString($"{_prefix}-{key}");

        if (!string.IsNullOrWhiteSpace(itemJson))
        {
            var cachedItem = JsonSerializer.Deserialize<TItem>(itemJson);
            return cachedItem;
        }

        var source = serviceProvider.GetRequiredService(options.CacheSourceType);
        var sourceGetMethod = options.SourceMethodSelector(source);
        var sourceItem = sourceGetMethod.Invoke(key, CancellationToken.None).Result;

        if (sourceItem is null)
        {
            return null;
        }

        itemJson = JsonSerializer.Serialize(sourceItem);

        cache.SetString($"{_prefix}-{key}", itemJson, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = options.ExpirationDuration,
        });

        return sourceItem;
    }

    /// <inheritdoc />
    public async Task<TItem?> GetAsync(string key, CancellationToken cancellationToken)
    {
        var itemJson = await cache.GetStringAsync($"{_prefix}-{key}", cancellationToken);

        if (!string.IsNullOrWhiteSpace(itemJson))
        {
            var cachedItem = JsonSerializer.Deserialize<TItem>(itemJson);
            return cachedItem;
        }

        var source = serviceProvider.GetRequiredService(options.CacheSourceType);
        var sourceGetMethod = options.SourceMethodSelector(source);
        var sourceItem = await sourceGetMethod.Invoke(key, cancellationToken);

        if (sourceItem is null)
        {
            return null;
        }

        itemJson = JsonSerializer.Serialize(sourceItem);

        await cache.SetStringAsync(
            $"{_prefix}-{key}",
            itemJson,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = options.ExpirationDuration },
            cancellationToken);

        return sourceItem;
    }

    /// <inheritdoc />
    public IEnumerable<TItem> Get(IEnumerable<string> keys)
    {
        foreach (var key in keys)
        {
            var item = Get(key);

            if (item is null)
            {
                continue;
            }

            yield return item;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TItem>> GetAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
    {
        var items = new LinkedList<TItem>();

        foreach (var key in keys)
        {
            var item = await GetAsync(key, cancellationToken);

            if (item is null)
            {
                continue;
            }

            items.AddLast(item);
        }

        return items;
    }

    /// <inheritdoc />
    public void Set(TItem item)
    {
        var itemJson = JsonSerializer.Serialize(item);

        cache.SetString($"{_prefix}-{options.KeySelector(item)}", itemJson, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = options.ExpirationDuration,
        });
    }

    /// <inheritdoc />
    public async Task SetAsync(TItem item, CancellationToken cancellationToken)
    {
        var itemJson = JsonSerializer.Serialize(item);

        await cache.SetStringAsync(
            $"{_prefix}-{options.KeySelector(item)}",
            itemJson,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = options.ExpirationDuration },
            cancellationToken);
    }

    /// <inheritdoc />
    public void Remove(string key)
    {
        cache.Remove($"{_prefix}-{key}");
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        await cache.RemoveAsync($"{_prefix}-{key}", cancellationToken);
    }

    /// <inheritdoc />
    public void Refresh(string key)
    {
        var source = serviceProvider.GetRequiredService(options.CacheSourceType);
        var sourceGetMethod = options.SourceMethodSelector(source);
        var sourceItem = sourceGetMethod.Invoke(key, CancellationToken.None).Result;

        if (sourceItem is null)
        {
            return;
        }

        Set(sourceItem);
    }

    /// <inheritdoc />
    public async Task RefreshAsync(string key, CancellationToken cancellationToken)
    {
        var source = serviceProvider.GetRequiredService(options.CacheSourceType);
        var sourceGetMethod = options.SourceMethodSelector(source);
        var sourceItem = await sourceGetMethod.Invoke(key, cancellationToken);

        if (sourceItem is null)
        {
            return;
        }

        await SetAsync(sourceItem, cancellationToken);
    }
}
