using AutoFixture;
using AutoFixture.AutoMoq;
using NanoWorks.Cache.Redis.Options;
using NanoWorks.Cache.Redis.Tests.TestObjects;
using NetTopologySuite.Index.HPRtree;
using Shouldly;

namespace NanoWorks.Cache.Redis.Tests.IntegrationTests;

/// <summary>
/// These tests require Redis running locally on the default port.
/// </summary>
public class RedisCacheTests
{
    private const int _oneThousandItems = 1000;
    private readonly TestCache _cache;
    private readonly IFixture _fixture = new Fixture();
    private readonly List<CacheTestItem> cacheTestItems = new List<CacheTestItem>();

    public RedisCacheTests()
    {
        var options = new CacheContextOptions<TestCache>();
        options.UseConnectionString("localhost:6379");
        _cache = new TestCache(options);

        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization() { ConfigureMembers = true });
        _fixture.CreateMany<CacheTestItem>(_oneThousandItems);

        foreach (var item in _fixture.CreateMany<CacheTestItem>(_oneThousandItems))
        {
            _cache.TestSet[item.Id] = item;
            cacheTestItems.Add(item);
        }
    }

    [Test]
    public void SingleItemIndexer_WhenItemExists_ReturnsItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet[item.Id] = item;

        // Act
        var result = _cache.TestSet[item.Id];

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(item);
    }

    [Test]
    public void SingleItemIndexer_WhenItemNotExists_ReturnsNull()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet[item.Id] = item;

        // Act
        var result = _cache.TestSet[Guid.NewGuid()];

        // Assert
        result.ShouldBeNull();
    }

    [Test]
    public void MultipleItemIndexer_WhenItemExists_ReturnsItems()
    {
        // Arrange
        var items = _fixture.CreateMany<CacheTestItem>();
        var ids = items.Select(x => x.Id);

        foreach (var item in items)
        {
            _cache.TestSet[item.Id] = item;
        }

        // Act
        var result = _cache.TestSet[ids];

        // Assert
        result.ShouldNotBeEmpty();
        result.Count().ShouldBe(items.Count());
        result.All(x => items.FirstOrDefault(y => y.Id == x.Id) != null).ShouldBeTrue();
    }

    [Test]
    public void MultipleItemIndexer_WhenItemNotExists_ReturnsEmptyCollection()
    {
        // Arrange
        var items = _fixture.CreateMany<CacheTestItem>();
        var ids = _fixture.CreateMany<Guid>();

        foreach (var item in items)
        {
            _cache.TestSet[item.Id] = item;
        }

        // Act
        var result = _cache.TestSet[ids];

        // Assert
        result.ShouldBeEmpty();
    }

    [Test]
    public void Remove_RemovesItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet[item.Id] = item;

        // Act
        _cache.TestSet.Remove(item.Id);

        // Assert
        var result = _cache.TestSet[item.Id];
        result.ShouldBeNull();
    }

    [Test]
    public async Task RemoveAsync_RemovesItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet[item.Id] = item;

        // Act
        await _cache.TestSet.RemoveAsync(item.Id);

        // Assert
        var result = _cache.TestSet[item.Id];
        result.ShouldBeNull();
    }

    [Test]
    public void GetKey_ReturnsKey()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet[item.Id] = item;

        // Act
        var key = _cache.TestSet.GetKey(item);

        // Assert
        key.ShouldBe(item.Id);
    }

    [Test]
    public void Count_ReturnsCount()
    {
        // act
        var count = _cache.TestSet.Count();

        // assert
        count.ShouldBeGreaterThan(0);
    }

    [Test]
    public void Enumerate_ReturnsItems()
    {
        // act
        var items = _cache.TestSet.ToList();

        // assert
        items.Count.ShouldBeGreaterThanOrEqualTo(_oneThousandItems);
    }
}
