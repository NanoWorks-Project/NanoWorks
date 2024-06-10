#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
// Ignore Spelling: Nano
// Ignore Spelling: Mq

namespace NanoWorks.Messaging.RabbitMq.Tests.TestObjects;

public sealed class TestMessageConsumer
{
    public static IEnumerable<TestSimpleMessage> SimpleMessages() => _simpleMessages;
    public static IEnumerable<TestComplexMessage> ComplexMessages() => _complexMessages;
    public static IEnumerable<TestExceptionMessage> ExceptionMessages() => _exceptionMessages;

    private static readonly ICollection<TestSimpleMessage> _simpleMessages = new LinkedList<TestSimpleMessage>();
    private static readonly ICollection<TestComplexMessage> _complexMessages = new LinkedList<TestComplexMessage>();
    private static readonly ICollection<TestExceptionMessage> _exceptionMessages = new LinkedList<TestExceptionMessage>();

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
}
