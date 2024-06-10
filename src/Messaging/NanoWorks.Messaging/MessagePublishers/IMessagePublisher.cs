// Ignore Spelling: Nano

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NanoWorks.Messaging.MessagePublishers
{
    /// <summary>
    /// Interface defining a message publisher.
    /// </summary>
    public interface IMessagePublisher : IDisposable
    {
        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <typeparam name="TMessage">Type of message to publish.</typeparam>
        /// <param name="message">Message to publish.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : class, new();
    }
}
