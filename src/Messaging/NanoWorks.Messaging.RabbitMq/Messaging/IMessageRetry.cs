// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System.Threading;
using System.Threading.Tasks;
using NanoWorks.Messaging.RabbitMq.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NanoWorks.Messaging.RabbitMq.Messaging;

/// <summary>
/// Marker interface for messages that should be retried.
/// </summary>
public interface IMessageRetry
{
    /// <summary>
    /// Retry the message.
    /// </summary>
    /// <param name="eventArgs"><see cref="BasicDeliverEventArgs"/>.</param>
    /// <param name="consumerOptions"><see cref="ConsumerOptions"/>.</param>
    /// <param name="channel"><see cref="IChannel"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RetryMessageAsync(BasicDeliverEventArgs eventArgs, ConsumerOptions consumerOptions, IChannel channel, CancellationToken cancellationToken);
}
