// Ignore Spelling: Nano

using AutoFixture;
using NanoWorks.Cache.InMemory.Options;

namespace NanoWorks.Cache.InMemory.Tests.UnitTests.Options;

public sealed class CashSetOptionsTests
{
    private readonly IFixture _fixture = new Fixture();

    [Test]
    public void Validate_WhenTableNameIsNullOrWhiteSpace_ThrowsInvalidOperationException()
    {
        // Arrange
        var options = _fixture.Create<InMemoryCashSetOptions>();
        options.TableName = null;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<InvalidOperationException>(Act, "CacheSet Table Name cannot be null or white-space");
    }

    [Test]
    public void Validate_WhenKeySelectorIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        var options = _fixture.Create<InMemoryCashSetOptions>();
        options.KeySelector = null;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<InvalidOperationException>(Act, "CacheSet Key is required");
    }

    [Test]
    public void Validate_WhenExpirationDurationIsLessThanOrEqualToZero_ThrowsInvalidOperationException()
    {
        // Arrange
        var options = _fixture.Create<InMemoryCashSetOptions>();
        options.ExpirationDuration = TimeSpan.Zero;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<InvalidOperationException>(Act, "CacheSet Expiration Duration is required");
    }
}
