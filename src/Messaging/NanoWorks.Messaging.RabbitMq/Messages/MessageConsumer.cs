// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NanoWorks.Messaging.Errors;
using NanoWorks.Messaging.RabbitMq.Options;
using NanoWorks.Messaging.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NanoWorks.Messaging.RabbitMq.Messages;

internal sealed class MessageConsumer : IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerOptions _consumerOptions;
    private readonly IChannel _channel;
    private readonly ILogger _logger;

    private AsyncEventingBasicConsumer _rabbitMqConsumer;
    private AsyncEventingBasicConsumer _rabbitMqRetryConsumer;

    internal MessageConsumer(IServiceProvider serviceProvider, ConsumerOptions consumerOptions, IChannel channel, ILogger logger)
    {
        _serviceProvider = serviceProvider;
        _consumerOptions = consumerOptions;
        _channel = channel;
        _logger = logger;
    }

    public void Dispose()
    {
        foreach (var consumerTag in _rabbitMqConsumer?.ConsumerTags ?? new string[0])
        {
            _channel.BasicCancelAsync(consumerTag).Wait();
        }

        foreach (var consumerTag in _rabbitMqRetryConsumer?.ConsumerTags ?? new string[0])
        {
            _channel.BasicCancelAsync(consumerTag).Wait();
        }
    }

    internal async Task StartAsync(CancellationToken cancellationToken)
    {
        _rabbitMqConsumer = new AsyncEventingBasicConsumer(_channel);
        _rabbitMqConsumer.ReceivedAsync += async (sender, eventArgs) => await TryProcessMessageAsync(eventArgs, cancellationToken);
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: _consumerOptions.MaxConcurrency, global: false, cancellationToken: cancellationToken);
        await _channel.BasicConsumeAsync(queue: _consumerOptions.QueueName, autoAck: false, consumer: _rabbitMqConsumer, cancellationToken: cancellationToken);

        _rabbitMqRetryConsumer = new AsyncEventingBasicConsumer(_channel);
        _rabbitMqRetryConsumer.ReceivedAsync += async (sender, eventArgs) => await TryProcessMessageAsync(eventArgs, cancellationToken);
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: _consumerOptions.MaxConcurrency, global: false, cancellationToken: cancellationToken);
        await _channel.BasicConsumeAsync(queue: _consumerOptions.RetryQueueName, autoAck: false, consumer: _rabbitMqRetryConsumer, cancellationToken: cancellationToken);
    }

    private async Task TryProcessMessageAsync(BasicDeliverEventArgs eventArgs, CancellationToken cancellationToken)
    {
        var messageProcessor = new MessageProcessor(_serviceProvider, _consumerOptions, _channel, eventArgs, _logger);

        try
        {
            await messageProcessor.ProcessMessageAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{_consumerOptions.ConsumerType.Name} failed to process message of type {eventArgs.BasicProperties.Type}.");
            var transportError = new TransportError(_consumerOptions.ConsumerType.FullName, ex);
            var transportErrorBody = MessageSerializer.Serialize(transportError);

            await _channel.BasicPublishAsync(
                exchange: typeof(TransportError).FullName,
                routingKey: string.Empty,
                mandatory: false,
                body: transportErrorBody,
                basicProperties: new BasicProperties { Type = typeof(TransportError).FullName, Persistent = true },
                cancellationToken: cancellationToken);

            if (_consumerOptions.MaxRetryCount > 0)
            {
                await messageProcessor.RetryMessageAsync(cancellationToken);
            }
        }
    }
}
