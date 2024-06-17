// Ignore Spelling: Nano

using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace NanoWorks.Cache.Redis.ConnectionPools
{
    internal static class ConnectionPool
    {
        private static readonly Queue<IConnectionMultiplexer> _connections = new Queue<IConnectionMultiplexer>();
        private static readonly object _lock = string.Empty;

        internal static int Size { get; set; } = Environment.ProcessorCount;

        /// <summary>
        /// Gets a connection from the pool.
        /// </summary>
        /// <param name="connectionString">Connection string for redis.</param>
        public static IConnectionMultiplexer GetConnection(string connectionString)
        {
            lock (_lock)
            {
                if (_connections.Count >= Size)
                {
                    var poolConnection = _connections.Dequeue();

                    if (poolConnection.IsConnected || poolConnection.IsConnecting)
                    {
                        _connections.Enqueue(poolConnection);
                        return poolConnection;
                    }

                    poolConnection.Dispose();
                }

                var newConnection = ConnectionMultiplexer.Connect(connectionString, options =>
                {
                    options.ConnectRetry = 5;
                    options.ConnectTimeout = 3000;
                    options.ReconnectRetryPolicy = new ExponentialRetry(options.ConnectTimeout);
                    options.KeepAlive = 5;
                    options.HeartbeatInterval = TimeSpan.FromSeconds(5);
                    options.AbortOnConnectFail = false;
                });

                _connections.Enqueue(newConnection);
                return newConnection;
            }
        }
    }
}
