// Ignore Spelling: Nano
// Ignore Spelling: Mq

using AutoFixture;
using NanoWorks.Messaging.RabbitMq.Options;

namespace NanoWorks.Messaging.RabbitMq.Tests.UnitTests.Options;

public sealed class MessagingOptionsTests
{
    private readonly IFixture _fixture = new Fixture();

    [Test]
    public void Validate_WhenConnectionStringIsNullOrWhiteSpace_ThrowsInvalidOperationException()
    {
        // arrange
        var options = _fixture.Create<MessagingOptions>();
        options.ConnectionString = string.Empty;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentNullException>(Act, $"Value cannot be null. (Parameter '{nameof(options.ConnectionString)}')");
    }

    [Test]
    public void Validate_WhenPublisherOptionsIsNull_ThrowsInvalidOperationException()
    {
        // arrange
        var options = _fixture.Create<MessagingOptions>();
        options.PublisherOptions = null;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentNullException>(Act, $"Value cannot be null. (Parameter '{nameof(options.PublisherOptions)}')");
    }

    [Test]
    public void Validate_WhenConsumerOptionsIsNull_ThrowsInvalidOperationException()
    {
        // arrange
        var options = _fixture.Create<MessagingOptions>();

        options.ConsumerOptions = new Dictionary<Type, ConsumerOptions>
        {
            { typeof(string), null! }
        };

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentNullException>(Act, $"Value cannot be null. (Parameter '{nameof(options.ConsumerOptions)}')");
    }
}
