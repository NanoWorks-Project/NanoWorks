// Ignore Spelling: Nano

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
    public sealed class RedisCacheSet<TItem, TKey> : ICacheSet<TItem, TKey>
        where TItem : class, new()
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        private readonly RedisCashSetOptions _options;

        internal RedisCacheSet(IConnectionMultiplexer connection, RedisCashSetOptions<TItem, TKey> options)
        {
            var isValidKey = typeof(TKey) == typeof(string)
                || typeof(TKey) == typeof(Guid)
                || typeof(TKey) == typeof(int)
                || typeof(TKey) == typeof(long);

            if (!isValidKey)
            {
                throw new InvalidOperationException($"Key must be a {typeof(string).Name}, {nameof(Guid)}, {typeof(int).Name}, or {typeof(long).Name}.");
            }

            _connection = connection;
            _database = _connection.GetDatabase();
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
            return item;
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

        /// <inheritdoc />
        public IReadOnlyList<TItem> ToList(int page = 0, int pageSize = 1000)
        {
            var keys = Keys(page, pageSize);
            var items = Get(keys);
            return items.ToList();
        }

        private IEnumerable<TItem> Get(IEnumerable<TKey> keys)
        {
            var redisKeys = keys
                .Where(x => !string.IsNullOrWhiteSpace(x?.ToString()))
                .Select(x => new RedisKey($"{_options.TableName}:{x}")).ToArray();

            var items = Get(redisKeys);
            return items;
        }

        private IEnumerable<TItem> Get(IEnumerable<RedisKey> keys)
        {
            if (!keys.Any())
            {
                yield break;
            }

            var redisResults = _database.JSON().MGet(keys.ToArray(), "$");

            foreach (var redisResult in redisResults)
            {
                if (redisResult.IsNull)
                {
                    continue;
                }

                var resultItems = JsonSerializer.Deserialize<TItem[]>(redisResult.ToString());

                if (resultItems.Length < 1)
                {
                    continue;
                }

                yield return resultItems[0];
            }
        }

        private IEnumerable<TItem> Items()
        {
            var keys = Keys();
            var items = Get(keys);
            return items;
        }

        private IEnumerable<RedisKey> Keys(int page = 0, int pageSize = int.MaxValue)
        {
            if (pageSize < 1)
            {
                yield break;
            }

            var endpoints = _connection.GetEndPoints();

            var keysReturned = 0;
            var currentIndex = 0;
            var itemsToSkip = page * pageSize;

            foreach (var endpoint in endpoints)
            {
                var server = _connection.GetServer(endpoint);

                foreach (var key in server.Keys(pattern: $"{_options.TableName}:*", pageSize: pageSize == int.MaxValue ? 1000 : pageSize))
                {
                    if (currentIndex < itemsToSkip)
                    {
                        currentIndex++;
                        continue;
                    }

                    if (keysReturned >= pageSize)
                    {
                        break;
                    }

                    keysReturned++;
                    yield return key;
                }
            }
        }
    }
}
