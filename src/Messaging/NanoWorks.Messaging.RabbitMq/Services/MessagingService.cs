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

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ExchangeHelper.CreateDefaultExchanges(_options.ConsumerConnection);
        ExchangeHelper.CreateMessageExchanges(_options.ConsumerConnection, _options.ConsumerOptions.Values);

        foreach (var subscriberOptions in _options.ConsumerOptions.Values)
        {
            var consumer = QueueHelper.Initialize(_serviceProvider, _options.ConsumerConnection, subscriberOptions);
            Consumers.Add(consumer);
        }

        foreach (var consumer in Consumers)
        {
            consumer.Start();
        }

        return Task.CompletedTask;
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
