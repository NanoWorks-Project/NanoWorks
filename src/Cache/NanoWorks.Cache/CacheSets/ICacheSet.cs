using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NanoWorks.Cache.CacheSets
{
    /// <summary>
    /// Cache set of items.
    /// </summary>
    /// <typeparam name="TItem">Type of item in the cache.</typeparam>
    /// <typeparam name="TKey">Type of key used to identify the item in the cache.</typeparam>
    public interface ICacheSet<TItem, TKey> : IEnumerable<TItem>, IEnumerable
        where TItem : class, new()
    {
        /// <summary>
        /// Returns the item with the specified key from the cache or null if the key does not exist.
        /// </summary>
        /// <param name="key">Item key.</param>
        TItem this[TKey key] { get; set; }

        /// <summary>
        /// Returns the items with the specified keys from the cache.
        /// </summary>
        /// <param name="keys">Item keys.</param>
        IEnumerable<TItem> this[IEnumerable<TKey> keys] { get; }

        /// <summary>
        /// Returns the number of items in the cache.
        /// </summary>
        long Count();

        /// <summary>
        /// Removes the item from the cache by key.
        /// </summary>
        /// <param name="key">Item key.</param>
        void Remove(TKey key);

        /// <summary>
        /// Removes the item from the cache by key.
        /// </summary>
        /// <param name="key">Item key.</param>
        Task RemoveAsync(TKey key);

        /// <summary>
        /// Returns the key for the specified item.
        /// </summary>
        /// <param name="item">Item to set in the cache.</param>
        TKey GetKey(TItem item);

        /// <summary>
        /// Resets the expiration of the cache set.
        /// </summary>
        void ResetExpiration();

        /// <summary>
        /// Resets the expiration of the cache set.
        /// </summary>
        Task ResetExpirationAsync();
    }
}
