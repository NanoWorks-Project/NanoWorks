// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NanoWorks.Messaging.RabbitMq.ConnectionPools;
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

    private readonly IConnectionPool _connectionPool;

    public MessagingService(
        IServiceProvider serviceProvider,
        MessagingOptions options,
        IConnectionPool connectionPool)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _connectionPool = connectionPool;
    }

    internal ICollection<MessageConsumer> Consumers { get; } = new LinkedList<MessageConsumer>();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var statupConnection = _connectionPool.GetConnection();
        using var startupChannel = await statupConnection.CreateChannelAsync(cancellationToken: cancellationToken);
        await RabbitMQHelper.CreateDefaultExchangesAsync(startupChannel, cancellationToken);
        await RabbitMQHelper.CreateDefaultQueuesAsync(startupChannel, cancellationToken);
        await RabbitMQHelper.CreateMessageExchangesAsync(startupChannel, _options.ConsumerOptions.Values, cancellationToken);

        foreach (var consumerOptions in _options.ConsumerOptions.Values)
        {
            var consumerConnection = _connectionPool.GetConnection();
            var consumerChannel = await consumerConnection.CreateChannelAsync(cancellationToken: cancellationToken);
            await RabbitMQHelper.CreateConsumerQueuesAsync(consumerOptions, consumerChannel, cancellationToken);
            var consumer = new MessageConsumer(_serviceProvider, consumerOptions, consumerChannel);

            Consumers.Add(consumer);
        }

        foreach (var consumer in Consumers)
        {
            await consumer.StartAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var consumer in Consumers)
        {
            await consumer.DisposeAsync();
        }
    }
}
