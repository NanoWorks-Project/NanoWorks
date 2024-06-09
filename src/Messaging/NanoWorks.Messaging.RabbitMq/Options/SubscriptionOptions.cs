using System;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable SA1402 // File may only contain a single type

namespace NanoWorks.Messaging.RabbitMq.Options
{
    /// <summary>
    /// Options for a message subscription.
    /// </summary>
    public class SubscriptionOptions
    {
        internal SubscriptionOptions()
        {
        }

        internal Type ConsumerType { get; set; }

        internal Type MessageType { get; set; }

        internal Func<object, Func<object, CancellationToken, Task>> ReceiveMethodSelector { get; set; }

        internal void Validate()
        {
            if (ConsumerType == null)
            {
                throw new ArgumentNullException(nameof(ConsumerType));
            }

            if (MessageType == null)
            {
                throw new ArgumentNullException(nameof(MessageType));
            }

            if (ReceiveMethodSelector == null)
            {
                throw new ArgumentNullException(nameof(ReceiveMethodSelector));
            }
        }
    }

    /// <summary>
    /// Options for a message subscription.
    /// </summary>
    /// <typeparam name="TConsumer">Type of consumer.</typeparam>
    /// <typeparam name="TMessage">Type of message.</typeparam>
    public class SubscriptionOptions<TConsumer, TMessage> : SubscriptionOptions
        where TMessage : class, new()
    {
        internal SubscriptionOptions(Func<TConsumer, Func<TMessage, CancellationToken, Task>> receiveMethodSelector)
        {
            ConsumerType = typeof(TConsumer);
            MessageType = typeof(TMessage);

            ReceiveMethodSelector = consumer =>
                (message, cancellationToken) => receiveMethodSelector((TConsumer)consumer)((TMessage)message, cancellationToken);
        }
    }
}
