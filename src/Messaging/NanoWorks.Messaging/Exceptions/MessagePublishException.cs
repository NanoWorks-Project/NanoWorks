// Ignore Spelling: Nano

using System;

namespace NanoWorks.Messaging.Exceptions
{
    /// <summary>
    /// Exception thrown when an error occurs while publishing a message.
    /// </summary>
    public class MessagePublishException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePublishException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public MessagePublishException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
