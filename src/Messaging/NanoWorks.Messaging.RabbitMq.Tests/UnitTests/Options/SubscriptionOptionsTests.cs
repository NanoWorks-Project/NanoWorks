// Ignore Spelling: Nano
// Ignore Spelling: Mq

using AutoFixture;
using NanoWorks.Messaging.RabbitMq.Options;

namespace NanoWorks.Messaging.RabbitMq.Tests.UnitTests.Options;

public sealed class SubscriptionOptionsTests
{
    private readonly IFixture _fixture = new Fixture();

    [Test]
    public void Validate_WhenConsumerTypeIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var options = _fixture.Create<SubscriptionOptions>();
        options.ConsumerType = null;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentNullException>(Act, $"Value cannot be null. (Parameter '{nameof(options.ConsumerType)}')");
    }

    [Test]
    public void Validate_WhenMessageTypeIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var options = _fixture.Create<SubscriptionOptions>();
        options.MessageType = null;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentNullException>(Act, $"Value cannot be null. (Parameter '{nameof(options.MessageType)}')");
    }

    [Test]
    public void Validate_WhenReceiveMethodSelectorIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var options = _fixture.Create<SubscriptionOptions>();
        options.ReceiveMethodSelector = null;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentNullException>(Act, $"Value cannot be null. (Parameter '{nameof(options.ReceiveMethodSelector)}')");
    }
}
