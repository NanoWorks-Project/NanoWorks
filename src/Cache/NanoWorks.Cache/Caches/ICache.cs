// Ignore Spelling: Nano

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NanoWorks.Cache.Caches;

/// <summary>
/// Distributed cache containing items of type <typeparamref name="TItem"/>.
/// </summary>
/// <typeparam name="TItem">Type of item in the cache.</typeparam>
public interface ICache<TItem>
    where TItem : class, new()
{
    /// <summary>
    /// Returns the item with the specified key from the cache or null if the key does not exist.
    /// </summary>
    /// <param name="key">Item key.</param>
    TItem? this[string key] { get; set; }

    /// <summary>
    /// Returns the items with the specified keys from the cache.
    /// </summary>
    /// <param name="keys">Item keys.</param>
    IEnumerable<TItem> this[IEnumerable<string> keys] { get; }

    /// <summary>
    /// Returns the item with the specified key from the cache or null if the key does not exist.
    /// </summary>
    /// <param name="key">Item key.</param>
    TItem? Get(string key);

    /// <summary>
    /// Returns the item with the specified key from the cache or null if the key does not exist.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<TItem?> GetAsync(string key, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the items with the specified keys from the cache or null if the key does not exist.
    /// </summary>
    /// <param name="keys">Item keys.</param>
    IEnumerable<TItem> Get(IEnumerable<string> keys);

    /// <summary>
    /// Returns the items with the specified keys from the cache or null if the key does not exist.
    /// </summary>
    /// <param name="keys">Item keys.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IEnumerable<TItem>> GetAsync(IEnumerable<string> keys, CancellationToken cancellationToken);

    /// <summary>
    /// Sets the item in the cache.
    /// </summary>
    /// <param name="item">The item.</param>
    void Set(TItem item);

    /// <summary>
    /// Sets the item in the cache.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetAsync(TItem item, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the item from the cache by key.
    /// </summary>
    /// <param name="key">Item key.</param>
    void Remove(string key);

    /// <summary>
    /// Removes the item from the cache by key.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the item from the source and sets it in the cache.
    /// </summary>
    /// <param name="key">Item key.</param>
    void Refresh(string key);

    /// <summary>
    /// Gets the item from the source and sets it in the cache.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RefreshAsync(string key, CancellationToken cancellationToken);
}
