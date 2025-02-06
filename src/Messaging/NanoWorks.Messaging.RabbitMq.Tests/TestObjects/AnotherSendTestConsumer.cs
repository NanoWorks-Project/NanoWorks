#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
// Ignore Spelling: Nano
// Ignore Spelling: Mq

using NanoWorks.Messaging.Errors;
using System.Collections.Concurrent;

namespace NanoWorks.Messaging.RabbitMq.Tests.TestObjects;

public sealed class AnotherSendTestConsumer
{
    public static IEnumerable<TestSimpleMessage> SimpleMessages() => _simpleMessages;
    public static IEnumerable<TestComplexMessage> ComplexMessages() => _complexMessages;
    public static IEnumerable<TestExceptionMessage> ExceptionMessages() => _exceptionMessages;
    public static IEnumerable<TransportError> TransportErrors() => _transportErrors;

    private static readonly ConcurrentBag<TestSimpleMessage> _simpleMessages = [];
    private static readonly ConcurrentBag<TestComplexMessage> _complexMessages = [];
    private static readonly ConcurrentBag<TestExceptionMessage> _exceptionMessages = [];
    private static readonly ConcurrentBag<TransportError> _transportErrors = [];

    public async Task ReceiveSimpleMessage(TestSimpleMessage message, CancellationToken cancellationToken)
    {
        _simpleMessages.Add(message);
    }

    public async Task ReceiveComplexMessage(TestComplexMessage message, CancellationToken cancellationToken)
    {
        _complexMessages.Add(message);
    }

    public async Task ReceiveExceptionMessage(TestExceptionMessage message, CancellationToken cancellationToken)
    {
        _exceptionMessages.Add(message);
        throw new NotImplementedException();
    }

    public async Task ReceiveTransportError(TransportError transportError, CancellationToken cancellationToken)
    {
        _transportErrors.Add(transportError);
    }
}
