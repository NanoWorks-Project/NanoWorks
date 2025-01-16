// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Collections.Generic;
using System.Linq;
using NanoWorks.Messaging.Errors;
using NanoWorks.Messaging.RabbitMq.Options;
using NanoWorks.Messaging.RabbitMq.Services;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.Helpers;

internal static class ExchangeHelper
{
    public static void CreateDefaultExchanges(IConnection connection)
    {
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: typeof(TransportError).FullName, type: ExchangeType.Fanout, durable: true, autoDelete: false);

        channel.QueueDeclare(MessagingService.DeadLetterQueueName, durable: true, exclusive: false, autoDelete: false, new Dictionary<string, object>
        {
            { "x-max-length",  1000 },
            { "x-message-ttl",  (int)TimeSpan.FromDays(1).TotalMilliseconds },
            { "queue-mode", "lazy" },
        });

        channel.ExchangeDeclare(MessagingService.DeadLetterExchangeName, ExchangeType.Fanout, durable: true, autoDelete: false);
        channel.QueueBind(MessagingService.DeadLetterQueueName, MessagingService.DeadLetterExchangeName, string.Empty);
    }

    public static void CreateMessageExchanges(IConnection connection, IEnumerable<ConsumerOptions> consumerOptions)
    {
        var channel = connection.CreateModel();

        foreach (var subscription in consumerOptions.SelectMany(x => x.Subscriptions.Values))
        {
            channel.ExchangeDeclare(exchange: subscription.MessageType.FullName, type: ExchangeType.Fanout, durable: true, autoDelete: false);
        }
    }
}
