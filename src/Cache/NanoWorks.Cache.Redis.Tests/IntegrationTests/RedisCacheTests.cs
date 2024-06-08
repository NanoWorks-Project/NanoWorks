using AutoFixture;
using AutoFixture.AutoMoq;
using NanoWorks.Cache.Redis.Options;
using NanoWorks.Cache.Redis.Tests.TestObjects;
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
            _cache.TestSet.Set(item);
            cacheTestItems.Add(item);
        }
    }

    [Test]
    public void SingleItemIndexer_WhenItemExists_ReturnsItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

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
        _cache.TestSet.Set(item);

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
            _cache.TestSet.Set(item);
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
            _cache.TestSet.Set(item);
        }

        // Act
        var result = _cache.TestSet[ids];

        // Assert
        result.ShouldBeEmpty();
    }

    [Test]
    public void Contains_WhenItemExists_ReturnsTrue()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

        // Act
        var result = _cache.TestSet.Contains(item.Id);

        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public void ContainsAsync_WhenItemExists_ReturnsTrue()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

        // Act
        var result = _cache.TestSet.ContainsAsync(item.Id).Result;

        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public void Contains_WhenItemNotExists_ReturnsFalse()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

        // Act
        var result = _cache.TestSet.Contains(Guid.NewGuid());

        // Assert
        result.ShouldBeFalse();
    }

    [Test]
    public async Task ContainsAsync_WhenItemNotExists_ReturnsFalse()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

        // Act
        var result = await _cache.TestSet.ContainsAsync(Guid.NewGuid());

        // Assert
        result.ShouldBeFalse();
    }

    [Test]
    public void Get_WhenItemExists_ReturnsItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

        // Act
        var result = _cache.TestSet.Get(item.Id);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(item);
    }

    [Test]
    public async Task GetAsync_WhenItemExists_ReturnsItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

        // Act
        var result = await _cache.TestSet.GetAsync(item.Id);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(item);
    }

    [Test]
    public void Get_WhenItemNotExists_ReturnsNull()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

        // Act
        var result = _cache.TestSet.Get(Guid.NewGuid());

        // Assert
        result.ShouldBeNull();
    }

    [Test]
    public async Task GetAsync_WhenItemNotExists_ReturnsNull()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

        // Act
        var result = await _cache.TestSet.GetAsync(Guid.NewGuid());

        // Assert
        result.ShouldBeNull();
    }

    [Test]
    public void Set_AddsItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();

        // Act
        _cache.TestSet.Set(item);

        // Assert
        var result = _cache.TestSet.Get(item.Id);
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(item);
    }

    [Test]
    public async Task SetAsync_AddsItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();

        // Act
        await _cache.TestSet.SetAsync(item);

        // Assert
        var result = await _cache.TestSet.GetAsync(item.Id);
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(item);
    }

    [Test]
    public void Set_UpdatesItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

        // Act
        item.Description = "Updated";
        _cache.TestSet.Set(item);

        // Assert
        var result = _cache.TestSet.Get(item.Id);
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(item);
    }

    [Test]
    public async Task SetAsync_UpdatesItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

        // Act
        item.Description = "Updated";
        await _cache.TestSet.SetAsync(item);

        // Assert
        var result = await _cache.TestSet.GetAsync(item.Id);
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(item);
    }

    [Test]
    public void Remove_ByKey_RemovesItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

        // Act
        _cache.TestSet.Remove(item.Id);

        // Assert
        var result = _cache.TestSet.Get(item.Id);
        result.ShouldBeNull();
    }

    [Test]
    public async Task RemoveAsync_ByKey_RemovesItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

        // Act
        await _cache.TestSet.RemoveAsync(item.Id);

        // Assert
        var result = await _cache.TestSet.GetAsync(item.Id);
        result.ShouldBeNull();
    }

    [Test]
    public void Remove_RemovesItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

        // Act
        _cache.TestSet.Remove(item);

        // Assert
        var result = _cache.TestSet.Get(item.Id);
        result.ShouldBeNull();
    }

    [Test]
    public async Task RemoveAsync_RemovesItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet.Set(item);

        // Act
        await _cache.TestSet.RemoveAsync(item);

        // Assert
        var result = await _cache.TestSet.GetAsync(item.Id);
        result.ShouldBeNull();
    }

    [Test]
    public async Task Enumerate_ReturnsItems()
    {
        // Arrange
        var items = _fixture.CreateMany<CacheTestItem>(10);

        foreach (var item in items)
        {
            await _cache.TestSet.SetAsync(item);
        }

        // Act
        var result = _cache.TestSet.ToList();

        // Assert
        result.ShouldNotBeNull();
        result.Any().ShouldBeTrue();

        foreach (var item in items)
        {
            var resultItem = result.FirstOrDefault(x => x.Id == item.Id);
            resultItem.ShouldNotBeNull();
            resultItem.ShouldBeEquivalentTo(item);
        }

        result.Count.ShouldBeGreaterThanOrEqualTo(_oneThousandItems);
    }
}
