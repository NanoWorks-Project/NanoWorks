using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NanoWorks.Cache.CacheSets;
using NanoWorks.Cache.Redis.Options;
using NanoWorks.Cache.Serializers;
using StackExchange.Redis;

namespace NanoWorks.Cache.Redis.CacheSets
{
    /// <summary>
    /// Redis cache set.
    /// </summary>
    /// <typeparam name="TItem">Type of item in the cache.</typeparam>
    /// <typeparam name="TKey">Type of key used to identify the item in the cache.</typeparam>
    public class CacheSet<TItem, TKey> : ICacheSet<TItem, TKey>
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
                throw new InvalidOperationException("Key must be a string, guid, int, or long.");
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
        public bool Contains(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _database.HashExists(_options.TableName, key.ToString());
        }

        /// <inheritdoc />
        public async Task<bool> ContainsAsync(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return await _database.HashExistsAsync(_options.TableName, key.ToString());
        }

        /// <inheritdoc />
        public long Count()
        {
            return _database.HashLength(_options.TableName);
        }

        /// <inheritdoc />
        public TItem Get(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var value = _database.HashGet(_options.TableName, key.ToString());

            if (!value.HasValue)
            {
                return null;
            }

            var item = CacheSerializer.Deserialize<TItem>(value, _options.SerializerExceptionBehavior);
            ResetExpiration();
            return item;
        }

        /// <inheritdoc />
        public IEnumerable<TItem> Get(IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                var item = Get(key);

                if (item != null)
                {
                    yield return item;
                }
            }
        }

        /// <inheritdoc />
        public async Task<TItem> GetAsync(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var value = await _database.HashGetAsync(_options.TableName, key.ToString());

            if (!value.HasValue)
            {
                return null;
            }

            var item = CacheSerializer.Deserialize<TItem>(value, _options.SerializerExceptionBehavior);
            await ResetExpirationAsync();
            return item;
        }

        /// <inheritdoc />
        public void Set(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var json = CacheSerializer.Serialize(item, _options.SerializerExceptionBehavior);

            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            var key = GetKey(item);

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _database.HashSet(_options.TableName, key.ToString(), json);
            ResetExpiration();
        }

        /// <inheritdoc />
        public async Task SetAsync(TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var json = CacheSerializer.Serialize(item, _options.SerializerExceptionBehavior);

            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            var key = GetKey(item);

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            await _database.HashSetAsync(_options.TableName, key.ToString(), json);
            await ResetExpirationAsync();
        }

        /// <inheritdoc />
        public void Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _database.HashDelete(_options.TableName, key.ToString());
        }

        /// <inheritdoc />
        public async Task RemoveAsync(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            await _database.HashDeleteAsync(_options.TableName, key.ToString());
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
        public void ResetExpiration()
        {
            _database.KeyExpire(_options.TableName, _options.ExpirationDuration);
        }

        /// <inheritdoc />
        public async Task ResetExpirationAsync()
        {
            await _database.KeyExpireAsync(_options.TableName, _options.ExpirationDuration);
        }

        /// <inheritdoc />
        public IEnumerator<TItem> GetEnumerator()
        {
            var result = _database.HashScan(_options.TableName);
            var enumerator = result.GetEnumerator();
            ResetExpiration();

            while (enumerator.MoveNext())
            {
                var redisResult = enumerator.Current;

                if (!redisResult.Value.HasValue)
                {
                    continue;
                }

                var item = CacheSerializer.Deserialize<TItem>(redisResult.Value, _options.SerializerExceptionBehavior);

                if (item is null)
                {
                    continue;
                }

                yield return item;
            }

            ResetExpiration();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
