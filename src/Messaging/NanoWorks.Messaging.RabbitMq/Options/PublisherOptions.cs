// Ignore Spelling: Nano
// Ignore Spelling: Mq

using NanoWorks.Messaging.Serialization;

namespace NanoWorks.Messaging.RabbitMq.Options;

/// <summary>
/// Options for the publisher.
/// </summary>
public sealed class PublisherOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to delete the exchange for this message type when the last subscriber / queue is unbound from it.
    /// </summary>
    public bool AutoDeleteExchange { get; set; } = false;

    /// <summary>
    /// Gets or sets the behavior for the serializer when an exception occurs.
    /// </summary>
    public PublisherSerializerExceptionBehavior SerializerExceptionBehavior { get; set; }

    /// <summary>
    /// Deletes the exchange for this message type when the last subscriber / queue is unbound from it.
    /// </summary>
    public PublisherOptions AutoDelete()
    {
        AutoDeleteExchange = true;
        return this;
    }

    /// <summary>
    /// Sets the behavior for the serializer when an exception occurs.
    /// If set to <see cref="PublisherSerializerExceptionBehavior.Throw"/>, the exception will be thrown.
    /// If set to <see cref="PublisherSerializerExceptionBehavior.Ignore"/>, the exception will be ignored and the message will not be published.
    /// </summary>
    /// <param name="behavior">Serializer behavior to use.</param>
    public PublisherOptions OnSerializationException(PublisherSerializerExceptionBehavior behavior)
    {
        SerializerExceptionBehavior = behavior;
        return this;
    }

    /// <summary>
    /// Validates the publisher options.
    /// </summary>
    public void Validate()
    {
    }
}
