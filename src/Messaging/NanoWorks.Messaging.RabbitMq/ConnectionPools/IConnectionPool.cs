// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.ConnectionPools;

/// <summary>
/// Connection pool for RabbitMQ.
/// </summary>
public interface IConnectionPool : IDisposable
{
    /// <summary>
    /// Gets a connection from the pool.
    /// </summary>
    IConnection GetConnection();
}