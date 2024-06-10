// Ignore Spelling: Nano
// Ignore Spelling: Mq

namespace NanoWorks.Messaging.RabbitMq.Tests.TestObjects;

public class TestComplexMessage
{
    public Guid Guid { get; set; }

    public IEnumerable<TestSimpleMessage> SimpleMessages { get; set; } = [];
}
