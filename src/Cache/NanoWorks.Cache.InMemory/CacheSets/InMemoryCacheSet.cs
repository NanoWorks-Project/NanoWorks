// Ignore Spelling: Nano

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NanoWorks.Cache.CacheSets;
using NanoWorks.Cache.InMemory.Options;

namespace NanoWorks.Cache.InMemory.CacheSets;

/// <summary>
/// In memory cache set.
/// </summary>
/// <typeparam name="TItem">Type of item in the cache.</typeparam>
/// <typeparam name="TKey">Type of key used to identify the item in the cache.</typeparam>
public sealed class InMemoryCacheSet<TItem, TKey> : ICacheSet<TItem, TKey>, IDisposable
    where TItem : class, new()
{
    private readonly InMemoryCashSetOptions _options;
    private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());

    internal InMemoryCacheSet(InMemoryCashSetOptions<TItem, TKey> options)
    {
        var isValidKey = typeof(TKey) == typeof(string)
            || typeof(TKey) == typeof(Guid)
            || typeof(TKey) == typeof(int)
            || typeof(TKey) == typeof(long);

        if (!isValidKey)
        {
            throw new InvalidOperationException($"Key must be a {typeof(string).Name}, {nameof(Guid)}, {typeof(int).Name}, or {typeof(long).Name}.");
        }

        _options = options;
    }

    /// <inheritdoc />
    public TItem this[TKey key]
    {
        get => Get(key);
        set => Set(value);
    }

    /// <inheritdoc />
    public IEnumerable<TItem> this[IEnumerable<TKey> keys]
    {
        get => Get(keys);
    }

    /// <inheritdoc />
    public TItem Get(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        var item = _memoryCache.Get<TItem>($"{_options.TableName}:{key}");
        return item;
    }

    /// <inheritdoc />
    public Task<TItem> GetAsync(TKey key)
    {
        var item = Get(key);
        return Task.FromResult(item);
    }

    /// <inheritdoc />
    public IEnumerable<TItem> Get(IEnumerable<TKey> keys)
    {
        keys = keys.Where(x => x != null);

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
    public async IAsyncEnumerable<TItem> GetAsync(IEnumerable<TKey> keys)
    {
        keys = keys.Where(x => x != null);

        foreach (var key in keys)
        {
            var item = await GetAsync(key);

            if (item is null)
            {
                continue;
            }

            yield return item;
        }
    }

    /// <inheritdoc />
    public IEnumerator<TItem> GetEnumerator()
    {
        var items = Items();
        return items.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public TKey GetKey(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var key = _options.KeySelector(item);
        return (TKey)_options.KeySelector(item);
    }

    /// <inheritdoc />
    public void Remove(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        _memoryCache.Remove($"{_options.TableName}:{key}");

        var keys = _memoryCache.Get<HashSet<TKey>>($"{_options.TableName}:keys") ?? [];
        keys.Remove(key);

        _memoryCache.Set($"{_options.TableName}:keys", keys, _options.ExpirationDuration);
    }

    /// <inheritdoc />
    public void Remove(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var key = GetKey(item);
        Remove(key);
    }

    /// <inheritdoc />
    public Task RemoveAsync(TKey key)
    {
        Remove(key);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(TItem item)
    {
        Remove(item);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void ResetExpiration(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        _memoryCache.Set($"{_options.TableName}:{key}", Get(key), _options.ExpirationDuration);
    }

    /// <inheritdoc />
    public void ResetExpiration(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var key = GetKey(item);
        ResetExpiration(key);
    }

    /// <inheritdoc />
    public Task ResetExpirationAsync(TKey key)
    {
        ResetExpiration(key);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ResetExpirationAsync(TItem item)
    {
        ResetExpiration(item);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Set(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var key = GetKey(item);
        _memoryCache.Set($"{_options.TableName}:{key}", item, _options.ExpirationDuration);

        var keys = _memoryCache.Get<HashSet<TKey>>($"{_options.TableName}:keys") ?? [];
        keys.Add(key);

        _memoryCache.Set($"{_options.TableName}:keys", keys, _options.ExpirationDuration);
    }

    /// <inheritdoc />
    public Task SetAsync(TItem item)
    {
        Set(item);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public IReadOnlyList<TItem> ToList(int page = 0, int pageSize = 1000)
    {
        var items = Items();
        return items.Skip(page * pageSize).Take(pageSize).ToList();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _memoryCache.Dispose();
    }

    private IEnumerable<TItem> Items()
    {
        var keys = _memoryCache.Get<HashSet<TKey>>($"{_options.TableName}:keys") ?? [];

        foreach (var key in keys)
        {
            var item = Get(key);

            if (item is null)
            {
                continue;
            }

            yield return Get(key);
        }
    }
}
