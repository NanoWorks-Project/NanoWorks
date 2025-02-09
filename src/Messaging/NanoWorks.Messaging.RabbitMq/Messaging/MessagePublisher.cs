// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NanoWorks.Messaging.Exceptions;
using NanoWorks.Messaging.MessagePublishers;
using NanoWorks.Messaging.RabbitMq.ConnectionPools;
using NanoWorks.Messaging.RabbitMq.Options;
using NanoWorks.Messaging.Serialization;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.Messaging;

internal sealed class MessagePublisher : IMessagePublisher
{
    private readonly MessagingOptions _options;
    private readonly IConnectionPool _connectionPool;
    private readonly IMessageSerializer _messageSerializer;
    private readonly ILogger<MessagePublisher> _logger;

    public MessagePublisher(
        MessagingOptions options,
        IConnectionPool connectionPool,
        IMessageSerializer messageSerializer,
        ILogger<MessagePublisher> logger)
    {
        _options = options;
        _connectionPool = connectionPool;
        _messageSerializer = messageSerializer;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task BroadcastAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, new()
    {
        try
        {
            var connection = _connectionPool.GetConnection();
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            var messageType = typeof(TMessage).FullName;
            var jsonBytes = _messageSerializer.Serialize(message);

            _logger.LogInformation("Broadcasting message of type {messageType}.", typeof(TMessage).Name);

            await channel.ExchangeDeclareAsync(
                exchange: messageType,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            await channel.BasicPublishAsync(
                exchange: messageType,
                routingKey: string.Empty,
                mandatory: false,
                body: jsonBytes,
                basicProperties: new BasicProperties { Type = messageType, Persistent = true },
                cancellationToken: cancellationToken);
        }
        catch (JsonException error)
        {
            _logger.LogError(error, "Error serializing message of type {messageType}.", typeof(TMessage).Name);

            if (_options.PublisherOptions.SerializerExceptionBehavior == PublisherSerializerExceptionBehavior.Throw)
            {
                throw;
            }
        }
        catch (Exception error)
        {
            _logger.LogError(error, "Error broadcasting message of type {messageType}", typeof(TMessage).Name);
            throw new MessagePublishException($"Error broadcasting message of type {typeof(TMessage).Name}.", error);
        }
    }

    /// <inheritdoc />
    public async Task SendAsync<TMessage>(string consumer, TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, new()
    {
        try
        {
            var connection = _connectionPool.GetConnection();
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            var messageType = typeof(TMessage).FullName;
            var jsonBytes = _messageSerializer.Serialize(message);

            _logger.LogInformation("Sending message of type {messageType} to {consumer}.", typeof(TMessage).Name, consumer);

            await channel.ExchangeDeclareAsync(
                exchange: messageType,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: consumer,
                mandatory: true,
                body: jsonBytes,
                basicProperties: new BasicProperties { Type = messageType, Persistent = true },
                cancellationToken: cancellationToken);
        }
        catch (JsonException error)
        {
            _logger.LogError(error, "Error serializing message of type {messageType}.", typeof(TMessage).Name);

            if (_options.PublisherOptions.SerializerExceptionBehavior == PublisherSerializerExceptionBehavior.Throw)
            {
                throw;
            }
        }
        catch (Exception error)
        {
            _logger.LogError(error, "Error sending message of type {messageType} to {consumer}.", typeof(TMessage).Name, consumer);
            throw new MessagePublishException($"Error sending message of type {typeof(TMessage).Name} to {consumer}.", error);
        }
    }
}
