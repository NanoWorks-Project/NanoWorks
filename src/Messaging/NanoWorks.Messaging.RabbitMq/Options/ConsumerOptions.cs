#pragma warning disable SA1402 // File may only contain a single type

// Ignore Spelling: Nano
// Ignore Spelling: Mq
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NanoWorks.Messaging.Errors;
using NanoWorks.Messaging.Serialization;

namespace NanoWorks.Messaging.RabbitMq.Options;

/// <summary>
/// Options for the message consumer.
/// </summary>
public class ConsumerOptions
{
    internal Type ConsumerType { get; set; }

    internal string QueueName { get; set; }

    internal string RetryQueueName { get; set; }

    internal TimeSpan Ttl { get; set; } = TimeSpan.Zero;

    internal int MaxCount { get; set; } = 0;

    internal ushort MaxConcurrency { get; set; } = 10;

    internal int MaxRetryCount { get; set; } = 3;

    internal TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    internal bool AutoDeleteQueue { get; set; } = false;

    internal ConsumerSerializerExceptionBehavior SerializerExceptionBehavior { get; set; }

    internal IDictionary<string, SubscriptionOptions> Subscriptions { get; set; } = new Dictionary<string, SubscriptionOptions>();

    /// <summary>
    /// Validates the consumer options.
    /// </summary>
    public void Validate()
    {
        if (ConsumerType == null)
        {
            throw new ArgumentNullException(nameof(ConsumerType));
        }

        if (string.IsNullOrWhiteSpace(QueueName))
        {
            throw new ArgumentNullException(nameof(QueueName));
        }

        if (string.IsNullOrWhiteSpace(RetryQueueName))
        {
            throw new ArgumentNullException(nameof(RetryQueueName));
        }

        if (Ttl < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(Ttl));
        }

        if (MaxCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(MaxCount));
        }

        if (MaxConcurrency < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(MaxConcurrency));
        }

        if (MaxRetryCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(MaxRetryCount));
        }

        if (RetryDelay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(RetryDelay));
        }

        if (MaxRetryCount > 0)
        {
            if (RetryDelay <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(RetryDelay));
            }
        }

        foreach (var subscription in Subscriptions.Values)
        {
            subscription.Validate();
        }
    }
}

/// <summary>
/// Options for the consumer.
/// </summary>
/// <typeparam name="TConsumer">Type of consumer.</typeparam>
public sealed class ConsumerOptions<TConsumer> : ConsumerOptions
    where TConsumer : class
{
    internal ConsumerOptions()
    {
        ConsumerType = typeof(TConsumer);
        QueueName = ConsumerType.FullName;
        RetryQueueName = $"{QueueName}.Retry";
    }

    /// <summary>
    /// Sets the queue name for the consumer.
    /// </summary>
    /// <param name="queueName">Name for the queue.</param>
    public void Queue(string queueName)
    {
        QueueName = queueName;
        RetryQueueName = $"{QueueName}.Retry";
    }

    /// <summary>
    /// Sets the time-to-live for messages in the queue. Once the TTL is reached, the message will be removed from the queue.
    /// </summary>
    /// <param name="messageTtl">Time-to-live for messages.</param>
    public void MessageTtl(TimeSpan messageTtl)
    {
        Ttl = messageTtl;
    }

    /// <summary>
    /// Sets the maximum number of messages that can be in the queue at any given time. Once the maximum is reached, no more messages will be accepted into the queue.
    /// </summary>
    /// <param name="maxCount">Maximum number of messages allowed in the queue.</param>
    public void MaxMessageCount(int maxCount)
    {
        MaxCount = maxCount;
    }

    /// <summary>
    /// Max number of messages that can be processed concurrently.
    /// </summary>
    /// <param name="maxConcurrency">Maximum number of messages that can be processed concurrently.</param>
    public void MaxMessageConcurrency(ushort maxConcurrency)
    {
        MaxConcurrency = maxConcurrency;
    }

    /// <summary>
    /// Sets the maximum number of times a message can be retried and the delay between each retry, before the message is moved to the dead-letter queue.
    /// </summary>
    /// <param name="maxRetryCount">Maximum number of times a message can be retried.</param>
    /// <param name="retryDelay">Delay between each retry.</param>
    public void Retries(int maxRetryCount, TimeSpan retryDelay)
    {
        MaxRetryCount = maxRetryCount;
        RetryDelay = retryDelay;
    }

    /// <summary>
    /// Deletes the associated queue if there are no consumers connected to it.
    /// </summary>
    public void AutoDelete()
    {
        AutoDeleteQueue = true;
    }

    /// <summary>
    /// If the serializer encounters an exception while serializing a message, this behavior will be used.
    /// If set to <see cref="ConsumerSerializerExceptionBehavior.Ignore"/>, the exception will be ignored and the message will be removed from the queue.
    /// If set to <see cref="ConsumerSerializerExceptionBehavior.DeadLetter"/>, the message will be moved to the dead-letter queue and a <see cref="TransportError"/> will be sent to the error queue.
    /// </summary>
    /// <param name="behavior">Behavior to use when a serializer exception occurs.</param>
    public void OnSerializationException(ConsumerSerializerExceptionBehavior behavior)
    {
        SerializerExceptionBehavior = behavior;
    }

    /// <summary>
    /// Subscribes to a message type.
    /// </summary>
    /// <typeparam name="TMessage">Type of message.</typeparam>
    /// <param name="endpointSelector">Function in the consumer to receive the message.</param>
    public void Subscribe<TMessage>(Func<TConsumer, Func<TMessage, CancellationToken, Task>> endpointSelector)
        where TMessage : class, new()
    {
        if (Subscriptions.ContainsKey(typeof(TMessage).FullName))
        {
            throw new InvalidOperationException($"Consumer {ConsumerType.FullName} is already subscribed to message type {typeof(TMessage).FullName}");
        }

        Subscriptions[typeof(TMessage).FullName] = new SubscriptionOptions<TConsumer, TMessage>(endpointSelector);
    }
}
