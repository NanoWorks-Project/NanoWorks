// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using RabbitMQ.Client.Events;

namespace NanoWorks.Messaging.RabbitMq.Messaging;

/// <summary>
/// Message serializer interface.
/// </summary>
public interface IMessageSerializer
{
    /// <summary>
    /// Deserialize message to the specified type.
    /// </summary>
    /// <param name="messageType">Message type.</param>
    /// <param name="eventArgs"><see cref="BasicDeliverEventArgs" />.</param>
    object Deserialize(Type messageType, BasicDeliverEventArgs eventArgs);

    /// <summary>
    /// Serializes the specified value to a byte array of utf8 characters.
    /// </summary>
    /// <typeparam name="TItem">Type of item to serialize.</typeparam>
    /// <param name="value">The value to serialize.</param>
    byte[] Serialize<TItem>(TItem value);
}
