// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Collections.Generic;
using NanoWorks.Messaging.RabbitMq.Options;
using NanoWorks.Messaging.RabbitMq.Services;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.Helpers
{
    internal static class ExchangeHelper
    {
        public static void CreateExchanges(IConnection connection)
        {
            var channel = connection.CreateModel();

            channel.QueueDeclare(MessagingService.ErrorQueueName, durable: true, exclusive: false, autoDelete: false, new Dictionary<string, object>
            {
                { "x-max-length",  1000 },
                { "x-message-ttl",  (int)TimeSpan.FromDays(1).TotalMilliseconds },
                { "queue-mode", "lazy" },
            });

            channel.QueueDeclare(MessagingService.DeadLetterQueueName, durable: true, exclusive: false, autoDelete: false, new Dictionary<string, object>
            {
                { "x-max-length",  1000 },
                { "x-message-ttl",  (int)TimeSpan.FromDays(1).TotalMilliseconds },
                { "queue-mode", "lazy" },
            });

            channel.ExchangeDeclare(MessagingService.ErrorQueueName, ExchangeType.Fanout, durable: true, autoDelete: false, null);
            channel.QueueBind(MessagingService.ErrorQueueName, MessagingService.ErrorQueueName, string.Empty, null);

            channel.ExchangeDeclare(MessagingService.DeadLetterExchangeName, ExchangeType.Fanout, durable: true, autoDelete: false, null);
            channel.QueueBind(MessagingService.DeadLetterQueueName, MessagingService.DeadLetterExchangeName, string.Empty, null);
        }
    }
}
