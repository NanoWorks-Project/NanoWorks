// Ignore Spelling: Nano

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NanoWorks.Cache.CacheSets;
using NanoWorks.Cache.Redis.Options;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace NanoWorks.Cache.Redis.CacheSets
{
    /// <summary>
    /// Redis cache set.
    /// </summary>
    /// <typeparam name="TItem">Type of item in the cache.</typeparam>
    /// <typeparam name="TKey">Type of key used to identify the item in the cache.</typeparam>
    public sealed class CacheSet<TItem, TKey> : ICacheSet<TItem, TKey>
        where TItem : class, new()
    {
        private readonly IDatabase _database;
        private readonly CashSetOptions _options;

        internal CacheSet(IDatabase database, CashSetOptions<TItem, TKey> options)
        {
            var isValidKey = typeof(TKey) == typeof(string)
                || typeof(TKey) == typeof(Guid)
                || typeof(TKey) == typeof(int)
                || typeof(TKey) == typeof(long);

            if (!isValidKey)
            {
                throw new InvalidOperationException($"Key must be a {typeof(string).Name}, {nameof(Guid)}, {typeof(int).Name}, or {typeof(long).Name}.");
            }

            _database = database;
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

            var item = _database.JSON().Get<TItem>($"{_options.TableName}:{key}");

            if (item != null)
            {
                ResetExpiration(key);
            }

            return item;
        }

        /// <inheritdoc />
        public async Task<TItem> GetAsync(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var item = await _database.JSON().GetAsync<TItem>($"{_options.TableName}:{key}");

            if (item != null)
            {
                await ResetExpirationAsync(key);
            }

            return item;
        }

        /// <inheritdoc />
        public TKey GetKey(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

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

            _database.KeyDelete($"{_options.TableName}:{key}");
        }

        /// <inheritdoc />
        public void Remove(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var key = GetKey(item);
            Remove(key);
        }

        /// <inheritdoc />
        public async Task RemoveAsync(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            await _database.KeyDeleteAsync($"{_options.TableName}:{key}");
        }

        /// <inheritdoc />
        public async Task RemoveAsync(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var key = GetKey(item);
            await RemoveAsync(key);
        }

        /// <inheritdoc />
        public void ResetExpiration(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _database.KeyExpire($"{_options.TableName}:{key}", _options.ExpirationDuration);
        }

        /// <inheritdoc />
        public void ResetExpiration(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var key = GetKey(item);
            ResetExpiration(key);
        }

        /// <inheritdoc />
        public async Task ResetExpirationAsync(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            await _database.KeyExpireAsync($"{_options.TableName}:{key}", _options.ExpirationDuration);
        }

        /// <inheritdoc />
        public async Task ResetExpirationAsync(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var key = GetKey(item);
            await ResetExpirationAsync(key);
        }

        /// <inheritdoc />
        public void Set(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var key = GetKey(item);
            _database.JSON().Set($"{_options.TableName}:{key}", "$", item);
            ResetExpiration(key);
        }

        /// <inheritdoc />
        public async Task SetAsync(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var key = GetKey(item);
            await _database.JSON().SetAsync($"{_options.TableName}:{key}", "$", item);
            await ResetExpirationAsync(key);
        }

        private IEnumerable<TItem> Get(IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                if (key == null)
                {
                    continue;
                }

                var item = Get(key);

                if (item == null)
                {
                    continue;
                }

                yield return item;
            }
        }
    }
}
