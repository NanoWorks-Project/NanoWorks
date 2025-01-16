// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NanoWorks.Messaging.RabbitMq.Helpers;
using NanoWorks.Messaging.RabbitMq.Messages;
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
        await ExchangeHelper.CreateDefaultExchangesAsync(_options.ConsumerConnection, cancellationToken);
        await ExchangeHelper.CreateMessageExchangesAsync(_options.ConsumerConnection, _options.ConsumerOptions.Values, cancellationToken);

        foreach (var subscriberOptions in _options.ConsumerOptions.Values)
        {
            var consumer = await QueueHelper.InitializeAsync(_serviceProvider, _options.ConsumerConnection, subscriberOptions, cancellationToken);
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
