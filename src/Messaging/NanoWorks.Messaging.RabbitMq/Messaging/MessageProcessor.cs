// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NanoWorks.Messaging.RabbitMq.Options;
using NanoWorks.Messaging.RabbitMq.TransportErrors;
using NanoWorks.Messaging.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NanoWorks.Messaging.RabbitMq.Messaging;

internal sealed class MessageProcessor
{
    private readonly ConsumerOptions _consumerOptions;
    private readonly IMessageSerializer _messageSerializer;
    private readonly object _messageEndpointInstance;
    private readonly ITransportErrorPublisher _transportErrorPublisher;
    private readonly IMessageRetry _messageRetry;
    private readonly ILogger<MessageProcessor> _logger;

    internal MessageProcessor(
        ConsumerOptions consumerOptions,
        IServiceScope serviceScope)
    {
        _consumerOptions = consumerOptions;
        _messageEndpointInstance = serviceScope.ServiceProvider.GetRequiredService(consumerOptions.ConsumerType);
        _messageSerializer = serviceScope.ServiceProvider.GetRequiredService<IMessageSerializer>();
        _transportErrorPublisher = serviceScope.ServiceProvider.GetRequiredService<ITransportErrorPublisher>();
        _messageRetry = serviceScope.ServiceProvider.GetRequiredService<IMessageRetry>();
        _logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<MessageProcessor>>();
    }

    public async Task ProcessMessageAsync(IChannel channel, BasicDeliverEventArgs eventArgs, CancellationToken cancellationToken)
    {
        var retryCount = eventArgs.BasicProperties.Headers?.TryGetValue("x-retry-count", out var value) ?? false ? (int)value : 0;
        var retryLimitReached = _consumerOptions.MaxRetryCount > 0 && retryCount >= _consumerOptions.MaxRetryCount;

        if (retryLimitReached)
        {
            await channel.BasicRejectAsync(eventArgs.DeliveryTag, requeue: false, cancellationToken);
            return;
        }

        var hasSubscription = _consumerOptions.Subscriptions.TryGetValue(eventArgs.BasicProperties.Type, out var subscription);

        if (!hasSubscription)
        {
            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken);
            return;
        }

        await InvokeMessageEndpointAsync(channel, eventArgs, subscription, cancellationToken);
    }

    private async Task InvokeMessageEndpointAsync(IChannel channel, BasicDeliverEventArgs eventArgs, SubscriptionOptions subscription, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("{consumerType} processing message of type {messageType}.", _consumerOptions.ConsumerType.Name, subscription.MessageType.Name);

            var message = _messageSerializer.Deserialize(subscription.MessageType, eventArgs);

            if (message is null)
            {
                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken);
                return;
            }

            var messageEndpoint = subscription.EndpointSelector(_messageEndpointInstance);
            await messageEndpoint.Invoke(message, cancellationToken);
            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken);

            _logger.LogInformation("{consumerType} finished processing message of type {messageType}.", _consumerOptions.ConsumerType.Name, subscription.MessageType.Name);
        }
        catch (JsonException error)
        {
            _logger.LogError(error, "Failed to deserialize message of type {messageType}.", subscription.MessageType.Name);

            if (_consumerOptions.SerializerExceptionBehavior == ConsumerSerializerExceptionBehavior.Ignore)
            {
                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken);
                return;
            }

            if (_consumerOptions.SerializerExceptionBehavior == ConsumerSerializerExceptionBehavior.DeadLetter)
            {
                await channel.BasicRejectAsync(eventArgs.DeliveryTag, requeue: false, cancellationToken);
            }

            throw;
        }
        catch (Exception error)
        {
            _logger.LogError(error, "{consumerType} failed to process message of type {messageType}.", _consumerOptions.ConsumerType.Name, subscription.MessageType.Name);
            await _transportErrorPublisher.PublishAsync(_consumerOptions.ConsumerType.FullName, error, channel, cancellationToken);

            if (_consumerOptions.MaxRetryCount > 0)
            {
                await _messageRetry.RetryMessageAsync(eventArgs, _consumerOptions, channel, cancellationToken);
            }
        }
    }
}
