// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NanoWorks.Messaging.RabbitMq.Messaging;

/// <summary>
/// Message consumer definition.
/// </summary>
public interface IMessageConsumer : IDisposable
{
    /// <summary>
    /// Start consuming messages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task StartAsync(CancellationToken cancellationToken);
}