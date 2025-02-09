// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NanoWorks.Messaging.RabbitMq.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NanoWorks.Messaging.RabbitMq.Messaging;

internal sealed class MessageRetry : IMessageRetry
{
    private readonly ILogger<MessageRetry> _logger;

    public MessageRetry(
        ILogger<MessageRetry> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task RetryMessageAsync(BasicDeliverEventArgs eventArgs, ConsumerOptions consumerOptions, IChannel channel, CancellationToken cancellationToken)
    {
        await Task.Delay(consumerOptions.RetryDelay, cancellationToken);

        _logger.LogInformation("{consumerType} retrying message of type {messageType}.", consumerOptions.ConsumerType.Name, eventArgs.BasicProperties.Type);

        var retryCount = eventArgs.BasicProperties.Headers?.TryGetValue("x-retry-count", out var value) ?? false ? (int)value : 0;

        var properties = new BasicProperties
        {
            Headers = eventArgs.BasicProperties.Headers ?? new Dictionary<string, object>(),
        };

        properties.Headers["x-retry-count"] = retryCount + 1;
        properties.Persistent = eventArgs.BasicProperties.Persistent;
        properties.Type = eventArgs.BasicProperties.Type;

        await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: consumerOptions.RetryQueueName,
            mandatory: true,
            basicProperties: properties,
            body: eventArgs.Body,
            cancellationToken: cancellationToken);
    }
}
