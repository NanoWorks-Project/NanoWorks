﻿using System;
using NanoWorks.Cache.Redis.CacheContexts;
using NanoWorks.Cache.Redis.ConnectionPools;
using StackExchange.Redis;

namespace NanoWorks.Cache.Redis.Options
{
    /// <summary>
    /// Options for the cache context.
    /// </summary>
    public class CacheContextOptions
    {
        internal string ConnectionString { get; set; }

        internal IConnectionMultiplexer Connection => ConnectionPool.GetConnection(ConnectionString);

        internal void Validate()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new InvalidOperationException("CacheContext Connection String cannot be null or white-space");
            }

            if (ConnectionPool.Size < 1)
            {
                throw new ArgumentOutOfRangeException("Connection Pool Size must be greater than 0");
            }
        }
    }

    /// <summary>
    /// Options for the cache context.
    /// </summary>
    /// <typeparam name="TCacheContext">Type of <see cref="RedisCacheContext"/>.</typeparam>
#pragma warning disable SA1402 // File may only contain a single type
    public class CacheContextOptions<TCacheContext> : CacheContextOptions
#pragma warning restore SA1402 // File may only contain a single type
        where TCacheContext : RedisCacheContext
    {
        /// <summary>
        /// Sets the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public void UseConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Sets the connection pool size.
        /// </summary>
        /// <param name="size">Maximum number of connections.</param>
        public void UseConnectionPoolSize(int size)
        {
            ConnectionPool.Size = size;
        }
    }
}