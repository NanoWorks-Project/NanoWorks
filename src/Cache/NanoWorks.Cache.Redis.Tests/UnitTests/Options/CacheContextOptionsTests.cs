// Ignore Spelling: Nano

using AutoFixture;
using NanoWorks.Cache.Redis.ConnectionPools;
using NanoWorks.Cache.Redis.Options;

namespace NanoWorks.Cache.Redis.Tests.UnitTests.Options;

public sealed class CacheContextOptionsTests
{
    private readonly IFixture _fixture = new Fixture();

    [Test]
    public void Validate_WhenConnectionStringIsNullOrWhiteSpace_ThrowsInvalidOperationException()
    {
        // Arrange
        var options = _fixture.Create<CacheContextOptions>();
        options.ConnectionString = null;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<InvalidOperationException>(Act, "CacheContext Connection String cannot be null or white-space");
    }

    [Test]
    public void Validate_WhenConnectionPoolSizeIsLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = _fixture.Create<CacheContextOptions>();
        options.ConnectionString = "test";
        ConnectionPool.Size = 0;

        // Act
        void Act() => options.Validate();

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act, "Connection Pool Size must be greater than 0");
    }
}
