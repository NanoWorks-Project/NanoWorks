// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NanoWorks.Messaging.Errors;
using NanoWorks.Messaging.RabbitMq.Options;
using NanoWorks.Messaging.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NanoWorks.Messaging.RabbitMq.Messages;

internal sealed class MessageProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerOptions _consumerOptions;
    private readonly IModel _channel;
    private readonly BasicDeliverEventArgs _eventArgs;
    private readonly ILogger _logger;

    internal MessageProcessor(
        IServiceProvider serviceProvider,
        ConsumerOptions consumerOptions,
        IModel channel,
        BasicDeliverEventArgs eventArgs,
        ILogger logger)
    {
        _serviceProvider = serviceProvider;
        _consumerOptions = consumerOptions;
        _channel = channel;
        _eventArgs = eventArgs;
        _logger = logger;
    }

    internal async Task ProcessMessageAsync()
    {
        var retryCount = _eventArgs.BasicProperties.Headers?.TryGetValue("x-retry-count", out var value) ?? false ? (int)value : 0;
        var retryLimitReached = _consumerOptions.MaxRetryCount > 0 && retryCount >= _consumerOptions.MaxRetryCount;

        if (retryLimitReached)
        {
            _channel.BasicReject(_eventArgs.DeliveryTag, requeue: false);
            return;
        }

        if (!_consumerOptions.Subscriptions.TryGetValue(_eventArgs.BasicProperties.Type, out var subscription))
        {
            // no subscription found for this message type
            _channel.BasicAck(_eventArgs.DeliveryTag, multiple: false);
            return;
        }

        object message = default;

        try
        {
            message = MessageSerializer.Deserialize(subscription.MessageType, _eventArgs.Body.Span);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, $"Failed to deserialize message of type {subscription.MessageType.FullName}.");

            if (_consumerOptions.SerializerExceptionBehavior == ConsumerSerializerExceptionBehavior.DeadLetter)
            {
                var transportError = new TransportError(_consumerOptions.ConsumerType.FullName, ex);
                var transportErrorBody = MessageSerializer.Serialize(transportError);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Type = typeof(TransportError).FullName;

                _channel.BasicPublish(exchange: properties.Type, routingKey: string.Empty, body: transportErrorBody, basicProperties: properties);
                return;
            }
        }

        if (message is null)
        {
            _channel.BasicAck(_eventArgs.DeliveryTag, multiple: false);
            return;
        }

        using (var scope = _serviceProvider.CreateScope())
        {
            var messageConsumer = scope.ServiceProvider.GetRequiredService(_consumerOptions.ConsumerType);
            var receiveMethod = subscription.ReceiveMethodSelector(messageConsumer);
            _logger.LogInformation($"{_consumerOptions.ConsumerType.Name} processing message of type {subscription.MessageType.Name}.");
            await receiveMethod.Invoke(message, CancellationToken.None);
            _channel.BasicAck(_eventArgs.DeliveryTag, multiple: false);
            _logger.LogInformation($"{_consumerOptions.ConsumerType.Name} finished processing message of type {subscription.MessageType.Name}.");
        }
    }

    internal async Task RetryMessageAsync()
    {
        _channel.BasicAck(_eventArgs.DeliveryTag, multiple: false);

        await Task.Delay(_consumerOptions.RetryDelay);

        var retryCount = _eventArgs.BasicProperties.Headers?.TryGetValue("x-retry-count", out var value) ?? false ? (int)value : 0;

        var properties = _channel.CreateBasicProperties();
        properties.Headers = _eventArgs.BasicProperties.Headers ?? new Dictionary<string, object>();
        properties.Headers["x-retry-count"] = retryCount + 1;
        properties.Persistent = _eventArgs.BasicProperties.Persistent;
        properties.Type = _eventArgs.BasicProperties.Type;

        _logger.LogInformation($"{_consumerOptions.ConsumerType.Name} retrying message of type {_eventArgs.BasicProperties.Type}.");
        _channel.BasicPublish(exchange: string.Empty, routingKey: _consumerOptions.RetryQueueName, properties, _eventArgs.Body);
    }
}
