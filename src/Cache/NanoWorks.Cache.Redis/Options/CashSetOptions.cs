#pragma warning disable SA1402 // File may only contain a single type
// Ignore Spelling: Nano

using System;
using NanoWorks.Cache.Options;

namespace NanoWorks.Cache.Redis.Options
{
    /// <summary>
    /// Options for a cache set.
    /// </summary>
    public class CashSetOptions
    {
        internal CashSetOptions()
        {
        }

        internal string TableName { get; set; }

        internal Func<object, object> KeySelector { get; set; }

        internal TimeSpan ExpirationDuration { get; set; } = TimeSpan.Zero;

        internal SerializerExceptionBehavior SerializerExceptionBehavior { get; set; } = SerializerExceptionBehavior.Ignore;

        internal void Validate()
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

        /// <summary>
        /// Behavior to use when a serializer exception occurs.
        /// </summary>
        /// <param name="behavior"><see cref="SerializerExceptionBehavior"/>.</param>
        public CashSetOptions<TItem, TKey> OnSerializationException(SerializerExceptionBehavior behavior)
        {
            SerializerExceptionBehavior = behavior;
            return this;
        }
    }
}
