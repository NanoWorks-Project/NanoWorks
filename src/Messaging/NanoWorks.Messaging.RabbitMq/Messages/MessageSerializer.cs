// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using RabbitMQ.Client.Events;

namespace NanoWorks.Messaging.RabbitMq.Messages;

internal sealed class MessageSerializer : IMessageSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
    };

    /// <inheritdoc />
    public object Deserialize(Type messageType, BasicDeliverEventArgs eventArgs)
    {
        var message = JsonSerializer.Deserialize(eventArgs.Body.Span, messageType, Options);
        return message;
    }

    /// <inheritdoc />
    public byte[] Serialize<TItem>(TItem value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return JsonSerializer.SerializeToUtf8Bytes(value, typeof(TItem), Options);
    }
}
