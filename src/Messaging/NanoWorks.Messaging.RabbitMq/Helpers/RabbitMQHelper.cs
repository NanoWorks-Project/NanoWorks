// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NanoWorks.Messaging.Errors;
using NanoWorks.Messaging.RabbitMq.Options;
using NanoWorks.Messaging.RabbitMq.Services;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.Helpers;

internal static class RabbitMQHelper
{
    public static async Task CreateDefaultExchangesAsync(IChannel channel, CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(
            exchange: typeof(TransportError).FullName,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: MessagingService.DeadLetterExchangeName,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);
    }

    public static async Task CreateDefaultQueuesAsync(IChannel channel, CancellationToken cancellationToken)
    {
        await channel.QueueDeclareAsync(
            queue: MessagingService.DeadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            new Dictionary<string, object>
            {
                { "x-max-length",  1000 },
                { "x-message-ttl",  (int)TimeSpan.FromDays(1).TotalMilliseconds },
                { "queue-mode", "lazy" },
            },
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: MessagingService.DeadLetterQueueName,
            exchange: MessagingService.DeadLetterExchangeName,
            routingKey: string.Empty,
            cancellationToken: cancellationToken);
    }

    public static async Task CreateMessageExchangesAsync(IChannel channel, IEnumerable<ConsumerOptions> consumerOptions, CancellationToken cancellationToken)
    {
        foreach (var subscription in consumerOptions.SelectMany(x => x.Subscriptions.Values))
        {
            await channel.ExchangeDeclareAsync(
                exchange: subscription.MessageType.FullName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);
        }
    }

    internal static async Task CreateConsumerQueuesAsync(ConsumerOptions consumerOptions, IChannel channel, CancellationToken cancellationToken)
    {
        var queueOptions = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", MessagingService.DeadLetterExchangeName },
            { "x-dead-letter-routing-key", MessagingService.DeadLetterQueueName },
            { "x-queue-mode", "lazy" },
        };

        if (consumerOptions.Ttl > TimeSpan.Zero)
        {
            queueOptions.Add("x-message-ttl", (int)consumerOptions.Ttl.TotalMilliseconds);
        }

        if (consumerOptions.MaxCount > 0)
        {
            queueOptions.Add("x-max-length", consumerOptions.MaxCount);
        }

        await channel.QueueDeclareAsync(
            queue: consumerOptions.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: consumerOptions.AutoDeleteQueue,
            queueOptions,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: consumerOptions.RetryQueueName,
            durable: true,
            exclusive: false,
            autoDelete: consumerOptions.AutoDeleteQueue,
            queueOptions,
            cancellationToken: cancellationToken);

        foreach (var subscription in consumerOptions.Subscriptions.Values)
        {
            await channel.QueueBindAsync(
                queue: consumerOptions.QueueName,
                exchange: subscription.MessageType.FullName,
                routingKey: string.Empty,
                cancellationToken: cancellationToken);
        }
    }
}
