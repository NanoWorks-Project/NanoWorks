#pragma warning disable SA1402 // File may only contain a single type
// Ignore Spelling: Nano

using System;

namespace NanoWorks.Cache.Redis.Options
{
    /// <summary>
    /// Options for a cache set.
    /// </summary>
    public class CashSetOptions
    {
        /// <summary>
        /// Gets or sets table name for the cache set.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets key selector for items in the cache set.
        /// </summary>
        public Func<object, object> KeySelector { get; set; }

        /// <summary>
        /// Gets or sets expiration duration for the cache set.
        /// </summary>
        public TimeSpan ExpirationDuration { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Validates the cache set options.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(TableName))
            {
                throw new InvalidOperationException("CacheSet Table Name cannot be null or white-space");
            }

            if (KeySelector == null)
            {
                throw new InvalidOperationException("CacheSet Key is required");
            }

            if (ExpirationDuration <= TimeSpan.Zero)
            {
                throw new InvalidOperationException("CacheSet Expiration Duration is required");
            }
        }
    }

    /// <summary>
    /// Options for a cache set.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the cache.</typeparam>
    /// <typeparam name="TKey">The type of the key for the cache set.</typeparam>
    public sealed class CashSetOptions<TItem, TKey> : CashSetOptions
        where TItem : class, new()
    {
        internal CashSetOptions()
        {
        }

        /// <summary>
        /// Table name for the cache set.
        /// </summary>
        /// <param name="tableName">The table name to use.</param>
        public CashSetOptions<TItem, TKey> Table(string tableName)
        {
            TableName = tableName;
            return this;
        }

        /// <summary>
        /// Sets the key selector for the cache set.
        /// </summary>
        /// <param name="keySelector">Function that returns the key for an item.</param>
        public CashSetOptions<TItem, TKey> Key(Func<TItem, TKey> keySelector)
        {
            KeySelector = item => keySelector((TItem)item);
            return this;
        }

        /// <summary>
        /// Sets the expiration duration for the cache set.
        /// </summary>
        /// <param name="expirationDuration">The expiration duration of the cache set.</param>
        public CashSetOptions<TItem, TKey> Expiration(TimeSpan expirationDuration)
        {
            ExpirationDuration = expirationDuration;
            return this;
        }
    }
}
