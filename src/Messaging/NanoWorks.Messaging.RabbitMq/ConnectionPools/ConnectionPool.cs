// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.ConnectionPools;

internal static class ConnectionPool
{
    private static readonly Queue<IConnection> _connections = new();

    internal static int Size { get; set; } = Environment.ProcessorCount;

    /// <summary>
    /// Gets a connection from the pool.
    /// </summary>
    /// <param name="connectionString">Connection string for RabbitMQ.</param>
    public static IConnection GetConnection(string connectionString)
    {
        if (_connections.Count >= Size)
        {
            var poolConnection = _connections.Dequeue();

            if (poolConnection.IsOpen)
            {
                _connections.Enqueue(poolConnection);
                return poolConnection;
            }

            poolConnection.Dispose();
        }

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString),
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(3),
            TopologyRecoveryEnabled = true,
            RequestedHeartbeat = TimeSpan.FromSeconds(5),
        };

        var newConnection = factory.CreateConnectionAsync().Result;
        _connections.Enqueue(newConnection);
        return newConnection;
    }
}
