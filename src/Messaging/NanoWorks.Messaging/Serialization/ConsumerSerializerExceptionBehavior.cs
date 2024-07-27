// Ignore Spelling: Nano

namespace NanoWorks.Messaging.Serialization;

/// <summary>
/// Specifies the behavior for handling exceptions thrown by the serializer.
/// </summary>
public enum ConsumerSerializerExceptionBehavior
{
    /// <summary>
    /// Ignore the exception and continue.
    /// </summary>
    Ignore,

    /// <summary>
    /// Move the message to the dead letter queue.
    /// </summary>
    DeadLetter,
}
