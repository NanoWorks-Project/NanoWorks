// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NanoWorks.Messaging.RabbitMq.Helpers;
using NanoWorks.Messaging.RabbitMq.Messaging;
using NanoWorks.Messaging.RabbitMq.Options;

namespace NanoWorks.Messaging.RabbitMq.Services;

internal sealed class MessagingService : IHostedService
{
    internal const string DeadLetterQueueName = "NanoWorks.DeadLetter";

    internal const string DeadLetterExchangeName = "NanoWorks.DeadLetterExchange";

    private readonly IServiceProvider _serviceProvider;

    private readonly MessagingOptions _options;

    public MessagingService(IServiceProvider serviceProvider, MessagingOptions options)
    {
        _serviceProvider = serviceProvider;
        _options = options;
    }

    internal ICollection<MessageConsumer> Consumers { get; } = new LinkedList<MessageConsumer>();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var startupChannel = await _options.ConsumerConnection.CreateChannelAsync(cancellationToken: cancellationToken);
        await RabbitMQHelper.CreateDefaultExchangesAsync(startupChannel, cancellationToken);
        await RabbitMQHelper.CreateDefaultQueuesAsync(startupChannel, cancellationToken);
        await RabbitMQHelper.CreateMessageExchangesAsync(startupChannel, _options.ConsumerOptions.Values, cancellationToken);

        foreach (var consumerOptions in _options.ConsumerOptions.Values)
        {
            var consumerChannel = await _options.ConsumerConnection.CreateChannelAsync(cancellationToken: cancellationToken);
            await RabbitMQHelper.CreateConsumerQueuesAsync(consumerOptions, consumerChannel, cancellationToken);
            var consumer = new MessageConsumer(_serviceProvider, consumerOptions, consumerChannel);

            Consumers.Add(consumer);
        }

        foreach (var consumer in Consumers)
        {
            await consumer.StartAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var consumer in Consumers)
        {
            consumer.Dispose();
        }

        return Task.CompletedTask;
    }
}
