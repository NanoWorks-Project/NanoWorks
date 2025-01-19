// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Threading;
using System.Threading.Tasks;
using NanoWorks.Messaging.Errors;
using NanoWorks.Messaging.RabbitMq.Messaging;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.TransportErrors;

internal sealed class TransportErrorPublisher : ITransportErrorPublisher
{
    private readonly IMessageSerializer _messageSerializer;

    public TransportErrorPublisher(IMessageSerializer messageSerializer)
    {
        _messageSerializer = messageSerializer;
    }

    /// <inheritdoc />
    public async Task PublishAsync(string consumer, Exception exception, IChannel channel, CancellationToken cancellationToken)
    {
        var transportError = new TransportError(consumer, exception);
        var transportErrorBody = _messageSerializer.Serialize(transportError);

        await channel.BasicPublishAsync(
            exchange: typeof(TransportError).FullName,
            routingKey: string.Empty,
            mandatory: false,
            body: transportErrorBody,
            basicProperties: new BasicProperties { Type = typeof(TransportError).FullName, Persistent = true },
            cancellationToken: cancellationToken);
    }
}
