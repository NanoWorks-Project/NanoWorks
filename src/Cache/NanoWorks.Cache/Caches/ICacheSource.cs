// Ignore Spelling: Nano

using System.Threading;
using System.Threading.Tasks;

namespace NanoWorks.Cache.Caches;

/// <summary>
/// Source of items for a cache.
/// </summary>
/// <typeparam name="TItem">Type of item in the cache.</typeparam>
public interface ICacheSource<TItem>
    where TItem : class, new()
{
    /// <summary>
    /// Gets the item with the specified key or null if the key does not exist.
    /// </summary>
    /// <param name="key">Key for the item.</param>
    TItem? Get(string key);

    /// <summary>
    /// Gets the item with the specified key or null if the key does not exist.
    /// </summary>
    /// <param name="key">Key for the item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<TItem?> GetAsync(string key, CancellationToken cancellationToken);
}
