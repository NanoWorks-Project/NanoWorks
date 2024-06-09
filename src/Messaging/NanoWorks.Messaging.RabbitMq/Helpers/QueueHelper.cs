using System;
using System.Collections.Generic;
using NanoWorks.Messaging.RabbitMq.Messages;
using NanoWorks.Messaging.RabbitMq.Options;
using NanoWorks.Messaging.RabbitMq.Services;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.Helpers
{
    internal static class QueueHelper
    {
        internal static MessageConsumer Initialize(IServiceProvider serviceProvider, IConnection connection, ConsumerOptions consumerOptions)
        {
            var channel = connection.CreateModel();

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

            channel.QueueDeclare(consumerOptions.QueueName, durable: true, exclusive: false, autoDelete: consumerOptions.AutoDeleteQueue, queueOptions);

            foreach (var subscription in consumerOptions.Subscriptions.Values)
            {
                channel.ExchangeDeclare(exchange: subscription.MessageType.FullName, type: ExchangeType.Fanout, durable: true, autoDelete: false);
                channel.QueueBind(consumerOptions.QueueName, exchange: subscription.MessageType.FullName, routingKey: string.Empty, null);
            }

            var consumer = new MessageConsumer(serviceProvider, consumerOptions, channel);
            return consumer;
        }
    }
}
