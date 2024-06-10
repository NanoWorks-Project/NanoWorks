// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.ConnectionPools
{
    internal static class ConnectionPool
    {
        private static readonly Queue<IConnection> _connections = new Queue<IConnection>();
        private static readonly object _lock = string.Empty;

        internal static int Size { get; set; } = Environment.ProcessorCount;

        /// <summary>
        /// Gets a connection from the pool.
        /// </summary>
        /// <param name="connectionString">Connection string for RabbitMQ.</param>
        public static IConnection GetConnection(string connectionString)
        {
            lock (_lock)
            {
                if (_connections.Count >= Size)
                {
                    var poolConnection = _connections.Dequeue();
                    _connections.Enqueue(poolConnection);
                    return poolConnection;
                }

                var factory = new ConnectionFactory
                {
                    Uri = new Uri(connectionString),
                };

                var newConnection = factory.CreateConnection();
                _connections.Enqueue(newConnection);
                return newConnection;
            }
        }
    }
}
