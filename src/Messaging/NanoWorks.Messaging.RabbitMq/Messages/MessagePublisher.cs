// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System.Threading;
using System.Threading.Tasks;
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

        public MessagePublisher(MessagingOptions options)
        {
            _channel = options.PublisherConnection.CreateModel();
            _options = options.PublisherOptions;
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
            var messageType = typeof(TMessage).FullName;
            var jsonBytes = MessageSerializer.Serialize(message, _options.SerializerExceptionBehavior);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Type = messageType;

            _channel.ExchangeDeclare(exchange: messageType, type: ExchangeType.Fanout, durable: true, autoDelete: _options.AutoDeleteExchange);
            _channel.BasicPublish(exchange: messageType, routingKey: string.Empty, body: jsonBytes, basicProperties: properties);
        }
    }
}
