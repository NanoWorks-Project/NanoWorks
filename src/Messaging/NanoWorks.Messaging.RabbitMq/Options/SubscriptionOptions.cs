﻿#pragma warning disable SA1402 // File may only contain a single type

// Ignore Spelling: Nano
// Ignore Spelling: Mq
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NanoWorks.Messaging.RabbitMq.Options;

/// <summary>
/// Options for a message subscription.
/// </summary>
public class SubscriptionOptions
{
    internal Type ConsumerType { get; set; }

    internal Type MessageType { get; set; }

    internal Func<object, Func<object, CancellationToken, Task>> EndpointSelector { get; set; }

    /// <summary>
    /// Validates the subscription options.
    /// </summary>
    public void Validate()
    {
        if (ConsumerType == null)
        {
            throw new ArgumentNullException(nameof(ConsumerType));
        }

        if (MessageType == null)
        {
            throw new ArgumentNullException(nameof(MessageType));
        }

        if (EndpointSelector == null)
        {
            throw new ArgumentNullException(nameof(EndpointSelector));
        }
    }
}

/// <summary>
/// Options for a message subscription.
/// </summary>
/// <typeparam name="TConsumer">Type of consumer.</typeparam>
/// <typeparam name="TMessage">Type of message.</typeparam>
public sealed class SubscriptionOptions<TConsumer, TMessage> : SubscriptionOptions
    where TMessage : class, new()
{
    internal SubscriptionOptions(Func<TConsumer, Func<TMessage, CancellationToken, Task>> endpointSelector)
    {
        ConsumerType = typeof(TConsumer);
        MessageType = typeof(TMessage);

        EndpointSelector = consumer =>
            (message, cancellationToken) => endpointSelector((TConsumer)consumer)((TMessage)message, cancellationToken);
    }
}
