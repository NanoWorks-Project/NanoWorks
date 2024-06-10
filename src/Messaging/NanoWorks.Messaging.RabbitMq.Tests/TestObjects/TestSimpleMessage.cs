// Ignore Spelling: Nano
// Ignore Spelling: Mq

namespace NanoWorks.Messaging.RabbitMq.Tests.TestObjects;

public class TestSimpleMessage
{
    public Guid Guid { get; set; }

    public string String { get; set; } = string.Empty;

    public int Integer { get; set; }

    public float Float { get; set; }

    public DateTime DateTime { get; set; }
}
