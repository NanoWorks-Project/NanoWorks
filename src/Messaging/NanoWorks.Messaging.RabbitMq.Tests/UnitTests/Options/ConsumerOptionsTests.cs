// Ignore Spelling: Nano
// Ignore Spelling: Mq

using AutoFixture;
using NanoWorks.Messaging.RabbitMq.Options;

namespace NanoWorks.Messaging.RabbitMq.Tests.UnitTests.Options;

public sealed class ConsumerOptionsTests
{
    private readonly IFixture _fixture = new Fixture();

    [Test]
    public void Validate_WhenConsumerTypeIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var options = _fixture.Create<ConsumerOptions>();
        options.ConsumerType = null;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentNullException>(Act, $"Value cannot be null. (Parameter '{nameof(options.ConsumerType)}')");
    }

    [Test]
    public void Validate_WhenQueueNameIsNullOrWhiteSpace_ThrowsArgumentNullException()
    {
        // Arrange
        var options = _fixture.Create<ConsumerOptions>();
        options.QueueName = null;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentNullException>(Act, $"Value cannot be null. (Parameter '{nameof(options.QueueName)}')");
    }

    [Test]
    public void Validate_WhenTtlIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = _fixture.Create<ConsumerOptions>();
        options.Ttl = TimeSpan.FromSeconds(-1);

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act, $"Specified argument was out of the range of valid values. (Parameter '{nameof(options.Ttl)}')");
    }

    [Test]
    public void Validate_WhenMaxCountIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = _fixture.Create<ConsumerOptions>();
        options.MaxCount = -1;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act, $"Specified argument was out of the range of valid values. (Parameter '{nameof(options.MaxCount)}')");
    }

    [Test]
    public void Validate_WhenMaxConcurrencyIsLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = _fixture.Create<ConsumerOptions>();
        options.MaxConcurrency = 0;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act, $"Specified argument was out of the range of valid values. (Parameter '{nameof(options.MaxConcurrency)}')");
    }

    [Test]
    public void Validate_WhenMaxRetryCountIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = _fixture.Create<ConsumerOptions>();
        options.MaxRetryCount = -1;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act, $"Specified argument was out of the range of valid values. (Parameter '{nameof(options.MaxRetryCount)}')");
    }

    [Test]
    public void Validate_WhenRetryDelayIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = _fixture.Create<ConsumerOptions>();
        options.RetryDelay = TimeSpan.FromSeconds(-1);

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act, $"Specified argument was out of the range of valid values. (Parameter '{nameof(options.RetryDelay)}')");
    }

    [Test]
    public void Validate_WhenMaxRetryCountIsGreaterThanZeroAndRetryDelayIsZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = _fixture.Create<ConsumerOptions>();
        options.MaxRetryCount = 1;
        options.RetryDelay = TimeSpan.Zero;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act, $"Specified argument was out of the range of valid values. (Parameter '{nameof(options.RetryDelay)}')");
    }
}
