// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Collections.Generic;
using NanoWorks.Messaging.RabbitMq.Options;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.ConnectionPools;

/// <summary>
/// Connection pool for RabbitMQ.
/// </summary>
public class ConnectionPool : IConnectionPool
{
    private readonly MessagingOptions _options;
    private readonly Queue<IConnection> _connections = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionPool"/> class.
    /// </summary>
    /// <param name="options">The messaging options to configure the connection pool.</param>
    public ConnectionPool(MessagingOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Disposes the connection pool.
    /// </summary>
    public void Dispose()
    {
        while (_connections.Count > 0)
        {
            var connection = _connections.Dequeue();
            connection.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets a connection from the pool.
    /// </summary>
    public IConnection GetConnection()
    {
        lock (_connections)
        {
            if (_connections.Count >= _options.ConnectionPoolSize)
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
                Uri = new Uri(_options.ConnectionString),
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
}