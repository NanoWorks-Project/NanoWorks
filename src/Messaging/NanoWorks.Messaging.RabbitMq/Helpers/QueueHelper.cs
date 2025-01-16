// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NanoWorks.Messaging.RabbitMq.Messages;
using NanoWorks.Messaging.RabbitMq.Options;
using NanoWorks.Messaging.RabbitMq.Services;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.Helpers;

internal static class QueueHelper
{
    internal static async Task<MessageConsumer> InitializeAsync(IServiceProvider serviceProvider, IConnection connection, ConsumerOptions consumerOptions, CancellationToken cancellationToken)
    {
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

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
            consumerOptions.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: consumerOptions.AutoDeleteQueue,
            queueOptions,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            consumerOptions.RetryQueueName,
            durable: true,
            exclusive: false,
            autoDelete: consumerOptions.AutoDeleteQueue,
            queueOptions,
            cancellationToken: cancellationToken);

        foreach (var subscription in consumerOptions.Subscriptions.Values)
        {
            await channel.ExchangeDeclareAsync(
                exchange: subscription.MessageType.FullName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await channel.QueueBindAsync(
                consumerOptions.QueueName,
                exchange: subscription.MessageType.FullName,
                routingKey: string.Empty,
                cancellationToken: cancellationToken);
        }

        var consumer = new MessageConsumer(serviceProvider, consumerOptions, channel, serviceProvider.GetRequiredService<ILogger<MessagingService>>());
        return consumer;
    }
}
