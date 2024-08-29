// Ignore Spelling: Nano

namespace NanoWorks.Messaging.Serialization;

/// <summary>
/// Specifies the behavior for handling exceptions thrown by the serializer.
/// </summary>
public enum PublisherSerializerExceptionBehavior
{
    /// <summary>
    /// Ignore the exception and continue.
    /// </summary>
    Ignore,

    /// <summary>
    /// Throw the exception.
    /// </summary>
    Throw,
}
