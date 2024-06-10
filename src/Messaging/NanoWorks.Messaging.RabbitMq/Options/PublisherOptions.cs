// Ignore Spelling: Nano
// Ignore Spelling: Mq

using NanoWorks.Messaging.Serialization;

namespace NanoWorks.Messaging.RabbitMq.Options
{
    /// <summary>
    /// Options for the publisher.
    /// </summary>
    public class PublisherOptions
    {
        internal bool AutoDeleteExchange { get; set; } = false;

        internal PublisherSerializerExceptionBehavior SerializerExceptionBehavior { get; set; }

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

        internal void Validate()
        {
        }
    }
}
