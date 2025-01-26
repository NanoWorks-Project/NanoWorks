// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NanoWorks.Messaging.RabbitMq.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NanoWorks.Messaging.RabbitMq.Messaging;

internal sealed class MessageConsumer : IMessageConsumer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerOptions _consumerOptions;
    private readonly IChannel _channel;

    private AsyncEventingBasicConsumer _rabbitMqConsumer;
    private AsyncEventingBasicConsumer _rabbitMqRetryConsumer;

    internal MessageConsumer(IServiceProvider serviceProvider, ConsumerOptions consumerOptions, IChannel channel)
    {
        _serviceProvider = serviceProvider;
        _consumerOptions = consumerOptions;
        _channel = channel;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var consumerTag in _rabbitMqConsumer?.ConsumerTags ?? [])
        {
            await _channel.BasicCancelAsync(consumerTag);
        }

        foreach (var consumerTag in _rabbitMqRetryConsumer?.ConsumerTags ?? [])
        {
            await _channel.BasicCancelAsync(consumerTag);
        }

        await _channel.DisposeAsync();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _rabbitMqConsumer = new AsyncEventingBasicConsumer(_channel);
        _rabbitMqConsumer.ReceivedAsync += async (sender, eventArgs) => await ConsumeMessageAsync(eventArgs, cancellationToken);
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: _consumerOptions.MaxConcurrency, global: false, cancellationToken: cancellationToken);
        await _channel.BasicConsumeAsync(queue: _consumerOptions.QueueName, autoAck: false, consumer: _rabbitMqConsumer, cancellationToken: cancellationToken);

        _rabbitMqRetryConsumer = new AsyncEventingBasicConsumer(_channel);
        _rabbitMqRetryConsumer.ReceivedAsync += async (sender, eventArgs) => await ConsumeMessageAsync(eventArgs, cancellationToken);
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: _consumerOptions.MaxConcurrency, global: false, cancellationToken: cancellationToken);
        await _channel.BasicConsumeAsync(queue: _consumerOptions.RetryQueueName, autoAck: false, consumer: _rabbitMqRetryConsumer, cancellationToken: cancellationToken);
    }

    private async Task ConsumeMessageAsync(BasicDeliverEventArgs eventArgs, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var messageProcessor = new MessageProcessor(_consumerOptions, scope);
        await messageProcessor.ProcessMessageAsync(_channel, eventArgs, cancellationToken);
    }
}
