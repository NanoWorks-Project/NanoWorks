// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.TransportErrors;

/// <summary>
/// Publishes transport errors.
/// </summary>
public interface ITransportErrorPublisher
{
    /// <summary>
    /// Publishes a transport error when a consumer throws an unexpected exception.
    /// </summary>
    /// <param name="consumer">Name of the consumer that encountered the exception.</param>
    /// <param name="exception">Exception that was thrown.</param>
    /// <param name="channel">Channel to publish the transport error.</param>
    /// <param name="cancellation">Cancellation token.</param>
    Task PublishAsync(string consumer, Exception exception, IChannel channel, CancellationToken cancellation);
}
