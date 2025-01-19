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

namespace NanoWorks.Messaging.RabbitMq.Messaging;

internal sealed class MessagePublisher : IMessagePublisher
{
    private readonly MessagingOptions _options;
    private readonly IMessageSerializer _messageSerializer;
    private readonly ILogger<MessagePublisher> _logger;
    private IChannel _channel;

    public MessagePublisher(MessagingOptions options, IMessageSerializer messageSerializer, ILogger<MessagePublisher> logger)
    {
        _options = options;
        _messageSerializer = messageSerializer;
        _logger = logger;
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }

    /// <inheritdoc />
    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, new()
    {
        try
        {
            _channel ??= await _options.PublisherConnection.CreateChannelAsync(cancellationToken: cancellationToken);
            var messageType = typeof(TMessage).FullName;
            var jsonBytes = _messageSerializer.Serialize(message);

            _logger.LogInformation($"Publishing message of type {typeof(TMessage).Name}.");

            await _channel.BasicPublishAsync(
                exchange: messageType,
                routingKey: string.Empty,
                mandatory: false,
                body: jsonBytes,
                basicProperties: new BasicProperties { Type = messageType, Persistent = true },
                cancellationToken: cancellationToken);
        }
        catch (JsonException error)
        {
            _logger.LogError(error, $"Error serializing message of type {typeof(TMessage).Name}.");

            if (_options.PublisherOptions.SerializerExceptionBehavior == PublisherSerializerExceptionBehavior.Throw)
            {
                throw;
            }
        }
        catch (Exception error)
        {
            var errorMessage = $"Error publishing message of type {typeof(TMessage).Name}.";
            _logger.LogError(error, errorMessage);
            throw new MessagePublishException(errorMessage, error);
        }
    }
}
