// Ignore Spelling: Nano
// Ignore Spelling: Mq

using NanoWorks.Messaging.Serialization;

namespace NanoWorks.Messaging.RabbitMq.Options;

/// <summary>
/// Options for the publisher.
/// </summary>
public sealed class PublisherOptions
{
    internal PublisherSerializerExceptionBehavior SerializerExceptionBehavior { get; set; }

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
