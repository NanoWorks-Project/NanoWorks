// Ignore Spelling: Nano

using System;

namespace NanoWorks.Messaging.Errors;

/// <summary>
/// Error that occurred during message transport.
/// </summary>
public class TransportError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransportError"/> class.
    /// </summary>
    public TransportError()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransportError"/> class.
    /// </summary>
    /// <param name="consumerName">Destination consumer that encountered the exception.</param>
    /// <param name="exception">Exception thrown by the consumer.</param>
    public TransportError(string consumerName, Exception exception)
    {
        ConsumerName = consumerName;
        Message = exception.Message;
        StackTrace = exception.StackTrace ?? string.Empty;
        InnerError = exception.InnerException != null ? new TransportError(consumerName, exception.InnerException) : null;
    }

    /// <summary>
    /// Gets or sets the name of the consumer that encountered the exception.
    /// </summary>
    public string ConsumerName { get; set; }

    /// <summary>
    /// Gets or sets message of the exception.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets stack trace of the exception.
    /// </summary>
    public string StackTrace { get; set; }

    /// <summary>
    /// Gets or sets inner exception.
    /// </summary>
    public TransportError InnerError { get; set; }
}
