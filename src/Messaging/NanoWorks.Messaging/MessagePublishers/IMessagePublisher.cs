// Ignore Spelling: Nano

using System.Threading;
using System.Threading.Tasks;

namespace NanoWorks.Messaging.MessagePublishers;

/// <summary>
/// Interface defining a message publisher.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Broadcasts a message to all subscribed consumers.
    /// </summary>
    /// <typeparam name="TMessage">Type of message to publish.</typeparam>
    /// <param name="message">Message to publish.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task BroadcastAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, new();

    /// <summary>
    /// Sends a message to a specific consumer.
    /// </summary>
    /// <param name="consumer">Consumer to send the message to.</param>
    /// <typeparam name="TMessage">Type of message to publish.</typeparam>
    /// <param name="message">Message to publish.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task SendAsync<TMessage>(string consumer, TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, new();
}
