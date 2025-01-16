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

internal static class ExchangeHelper
{
    public static async Task CreateDefaultExchangesAsync(IConnection connection, CancellationToken cancellationToken)
    {
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: typeof(TransportError).FullName,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            MessagingService.DeadLetterQueueName,
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

        await channel.ExchangeDeclareAsync(
            MessagingService.DeadLetterExchangeName,
            ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            MessagingService.DeadLetterQueueName,
            MessagingService.DeadLetterExchangeName,
            routingKey: string.Empty,
            cancellationToken: cancellationToken);
    }

    public static async Task CreateMessageExchangesAsync(IConnection connection, IEnumerable<ConsumerOptions> consumerOptions, CancellationToken cancellationToken)
    {
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

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
}
