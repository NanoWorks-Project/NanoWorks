// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NanoWorks.Messaging.Exceptions;
using NanoWorks.Messaging.MessagePublishers;
using NanoWorks.Messaging.RabbitMq.Options;
using NanoWorks.Messaging.Serialization;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.Messages
{
    internal sealed class MessagePublisher : IMessagePublisher
    {
        private readonly IModel _channel;
        private readonly PublisherOptions _options;
        private readonly ILogger _logger;

        public MessagePublisher(MessagingOptions options, ILogger<MessagePublisher> logger)
        {
            _channel = options.PublisherConnection.CreateModel();
            _options = options.PublisherOptions;
            _logger = logger;
        }

        public void Dispose()
        {
            _channel.Dispose();
        }

        /// <inheritdoc />
        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : class, new()
        {
            var publishTask = Task.Run(() => Publish(message), cancellationToken);
            await publishTask;
        }

        private void Publish<TMessage>(TMessage message)
            where TMessage : class, new()
        {
            try
            {
                PublishMessage(message);
            }
            catch (JsonException ex)
            {
                var errorMessage = $"Error serializing message of type {typeof(TMessage).Name}.";
                _logger.LogError(ex, errorMessage);

                if (_options.SerializerExceptionBehavior == PublisherSerializerExceptionBehavior.Throw)
                {
                    throw new MessagePublishException(errorMessage, ex);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error publishing message of type {typeof(TMessage).Name}.";
                _logger.LogError(ex, errorMessage);
                throw new MessagePublishException(errorMessage, ex);
            }
        }

        private void PublishMessage<TMessage>(TMessage message)
            where TMessage : class, new()
        {
            var messageType = typeof(TMessage).FullName;
            var jsonBytes = MessageSerializer.Serialize(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Type = messageType;

            _logger.LogInformation($"Publishing message of type {typeof(TMessage).Name}.");
            _channel.ExchangeDeclare(exchange: messageType, type: ExchangeType.Fanout, durable: true, autoDelete: _options.AutoDeleteExchange);
            _channel.BasicPublish(exchange: messageType, routingKey: string.Empty, body: jsonBytes, basicProperties: properties);
        }
    }
}
