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
                    _connections.Enqueue(poolConnection);
                    return poolConnection;
                }

                var newConnection = ConnectionMultiplexer.Connect(connectionString);
                _connections.Enqueue(newConnection);
                return newConnection;
            }
        }
    }
}
