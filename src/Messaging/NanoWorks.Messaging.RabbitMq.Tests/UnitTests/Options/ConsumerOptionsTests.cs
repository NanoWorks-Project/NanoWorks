// Ignore Spelling: Nano
// Ignore Spelling: Mq

using NanoWorks.Messaging.RabbitMq.Options;

namespace NanoWorks.Messaging.RabbitMq.Tests.UnitTests.Options;

public sealed class ConsumerOptionsTests
{
    [Test]
    public void Validate_WhenConsumerTypeIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var options = CreateConsumerOptions();
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
        var options = CreateConsumerOptions();
        options.QueueName = null;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentNullException>(Act, $"Value cannot be null. (Parameter '{nameof(options.QueueName)}')");
    }

    [Test]
    public void Validate_WhenRetryQueueNameIsNullOrWhiteSpace_ThrowsArgumentNullException()
    {
        // Arrange
        var options = CreateConsumerOptions();
        options.RetryQueueName = null;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentNullException>(Act, $"Value cannot be null. (Parameter '{nameof(options.RetryQueueName)}')");
    }

    [Test]
    public void Validate_WhenTtlIsLessThanZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = CreateConsumerOptions();
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
        var options = CreateConsumerOptions();
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
        var options = CreateConsumerOptions();
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
        var options = CreateConsumerOptions();
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
        var options = CreateConsumerOptions();
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
        var options = CreateConsumerOptions();
        options.MaxRetryCount = 1;
        options.RetryDelay = TimeSpan.Zero;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act, $"Specified argument was out of the range of valid values. (Parameter '{nameof(options.RetryDelay)}')");
    }

    private ConsumerOptions CreateConsumerOptions()
    {
        var options = new ConsumerOptions();
        options.ConsumerType = typeof(ConsumerOptionsTests);
        options.QueueName = "queue";
        options.RetryQueueName = "retry";
        options.Ttl = TimeSpan.FromSeconds(1);
        options.MaxCount = 1;
        options.MaxConcurrency = 1;
        options.MaxRetryCount = 1;
        options.RetryDelay = TimeSpan.FromSeconds(1);
        options.Subscriptions.Add(nameof(ConsumerOptions), new SubscriptionOptions());
        return options;
    }
}
