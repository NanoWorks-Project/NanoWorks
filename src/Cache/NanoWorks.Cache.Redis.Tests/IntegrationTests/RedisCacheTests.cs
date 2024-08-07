﻿// Ignore Spelling: Nano

using AutoFixture;
using AutoFixture.AutoMoq;
using NanoWorks.Cache.Redis.Options;
using NanoWorks.Cache.Redis.Tests.TestObjects;
using Shouldly;

namespace NanoWorks.Cache.Redis.Tests.IntegrationTests;

/// <summary>
/// These tests require Redis running locally on the default port.
/// </summary>
public sealed class RedisCacheTests
{
    private readonly TestCache _cache;
    private readonly Fixture _fixture;
    private readonly List<CacheTestItem> _items;

    public RedisCacheTests()
    {
        var options = new RedisCashSetOptions<TestCache>();
        options.UseConnectionString("localhost:6379");
        _cache = new TestCache(options);

        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization() { ConfigureMembers = true });

        _items = _fixture.CreateMany<CacheTestItem>(1000).ToList();

        foreach (var item in _items)
        {
            _cache.TestSet[item.Id] = item;
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
    public void Get_WhenItemExists_ReturnsItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet[item.Id] = item;

        // Act
        var result = _cache.TestSet.Get(item.Id);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(item);
    }

    [Test]
    public void Get_WhenItemNotExists_ReturnsNull()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        // Act
        var result = _cache.TestSet.Get(id);

        // Assert
        result.ShouldBeNull();
    }

    [Test]
    public async Task GetAsync_WhenItemExists_ReturnsItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet[item.Id] = item;

        // Act
        var result = await _cache.TestSet.GetAsync(item.Id);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(item);
    }

    [Test]
    public async Task GetAsync_WhenItemNotExists_ReturnsNull()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        // Act
        var result = await _cache.TestSet.GetAsync(id);

        // Assert
        result.ShouldBeNull();
    }

    [Test]
    public void Get_MultipleKeys_ReturnsItems()
    {
        // Arrange
        var items = _fixture.CreateMany<CacheTestItem>();

        foreach (var item in items)
        {
            _cache.TestSet[item.Id] = item;
        }

        var ids = items.Select(x => x.Id);

        // Act
        var result = _cache.TestSet.Get(ids);

        // Assert
        result.ShouldNotBeEmpty();
    }

    [Test]
    public async Task GetAsync_MultipleKeys_ReturnsItems()
    {
        // Arrange
        var items = _fixture.CreateMany<CacheTestItem>();

        foreach (var item in items)
        {
            _cache.TestSet[item.Id] = item;
        }

        var ids = items.Select(x => x.Id);

        // Act
        await foreach (var item in _cache.TestSet.GetAsync(ids))
        {
            item.ShouldNotBeNull();
        }
    }

    [Test]
    public void GetKey_WhenItemExists_ReturnsKey()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();

        // Act
        var result = _cache.TestSet.GetKey(item);

        // Assert
        result.ShouldBe(item.Id);
    }

    [Test]
    public void GetKey_WhenItemNotExists_ThrowsArgumentNullException()
    {
        // Arrange
        CacheTestItem item = null!;

        // Act
        Should.Throw<ArgumentNullException>(() => _cache.TestSet.GetKey(item));
    }

    [Test]
    public void Remove_WhenItemExists_RemovesItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet[item.Id] = item;

        // Act
        _cache.TestSet.Remove(item.Id);

        // Assert
        var result = _cache.TestSet.Get(item.Id);
        result.ShouldBeNull();
    }

    [Test]
    public void Remove_WhenItemNotExists_DoesNothing()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        // Act
        _cache.TestSet.Remove(id);

        // Assert
        var result = _cache.TestSet.Get(id);
        result.ShouldBeNull();
    }

    [Test]
    public void Remove_WhenItemExists_RemovesItemByItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet[item.Id] = item;

        // Act
        _cache.TestSet.Remove(item);

        // Assert
        var result = _cache.TestSet.Get(item.Id);
        result.ShouldBeNull();
    }

    [Test]
    public void Remove_WhenItemNotExists_DoesNothingByItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();

        // Act
        _cache.TestSet.Remove(item);

        // Assert
        var result = _cache.TestSet.Get(item.Id);
        result.ShouldBeNull();
    }

    [Test]
    public void Remove_WhenItemIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        CacheTestItem item = null!;

        // Act
        Should.Throw<ArgumentNullException>(() => _cache.TestSet.Remove(item));
    }

    [Test]
    public void Remove_WhenKeyIsNull_ThrowsArgumentNullException()
    {
        // Act
        Should.Throw<ArgumentNullException>(() => _cache.TestSet.Remove(null!));
    }

    [Test]
    public async Task RemoveAsync_WhenItemExists_RemovesItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet[item.Id] = item;

        // Act
        await _cache.TestSet.RemoveAsync(item.Id);

        // Assert
        var result = _cache.TestSet.Get(item.Id);
        result.ShouldBeNull();
    }

    [Test]
    public async Task RemoveAsync_WhenItemNotExists_DoesNothing()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        // Act
        await _cache.TestSet.RemoveAsync(id);

        // Assert
        var result = _cache.TestSet.Get(id);
        result.ShouldBeNull();
    }

    [Test]
    public async Task RemoveAsync_WhenItemExists_RemovesItemByItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet[item.Id] = item;

        // Act
        await _cache.TestSet.RemoveAsync(item);

        // Assert
        var result = _cache.TestSet.Get(item.Id);
        result.ShouldBeNull();
    }

    [Test]
    public async Task RemoveAsync_WhenItemNotExists_DoesNothingByItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();

        // Act
        await _cache.TestSet.RemoveAsync(item);

        // Assert
        var result = _cache.TestSet.Get(item.Id);
        result.ShouldBeNull();
    }

    [Test]
    public async Task RemoveAsync_WhenItemIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        CacheTestItem item = null!;

        // Act
        await Should.ThrowAsync<ArgumentNullException>(() => _cache.TestSet.RemoveAsync(item));
    }

    [Test]
    public async Task RemoveAsync_WhenKeyIsNull_ThrowsArgumentNullException()
    {
        // Act
        await Should.ThrowAsync<ArgumentNullException>(() => _cache.TestSet.RemoveAsync(null!));
    }

    [Test]
    public void Set_WhenItemExists_UpdatesItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet[item.Id] = item;

        var updatedItem = _fixture.Create<CacheTestItem>();
        updatedItem.Id = item.Id;

        // Act
        _cache.TestSet.Set(updatedItem);

        // Assert
        var result = _cache.TestSet.Get(item.Id);
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(updatedItem);
    }

    [Test]
    public void Set_WhenItemNotExists_AddsItem()
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
    public void Set_WhenItemIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        CacheTestItem item = null!;

        // Act
        Should.Throw<ArgumentNullException>(() => _cache.TestSet.Set(item));
    }

    [Test]
    public async Task SetAsync_WhenItemExists_UpdatesItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();
        _cache.TestSet[item.Id] = item;

        var updatedItem = _fixture.Create<CacheTestItem>();
        updatedItem.Id = item.Id;

        // Act
        await _cache.TestSet.SetAsync(updatedItem);

        // Assert
        var result = _cache.TestSet.Get(item.Id);
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(updatedItem);
    }

    [Test]
    public async Task SetAsync_WhenItemNotExists_AddsItem()
    {
        // Arrange
        var item = _fixture.Create<CacheTestItem>();

        // Act
        await _cache.TestSet.SetAsync(item);

        // Assert
        var result = _cache.TestSet.Get(item.Id);
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(item);
    }

    [Test]
    public async Task SetAsync_WhenItemIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        CacheTestItem item = null!;

        // Act
        await Should.ThrowAsync<ArgumentNullException>(() => _cache.TestSet.SetAsync(item));
    }

    [Test]
    public void ToList_ReturnsAllItems()
    {
        // act
        var results = _cache.TestSet.ToList();

        // assert
        results.ShouldNotBeEmpty();
        results.Count.ShouldBe(_items.Count);
    }

    [TestCase(0, 100)]
    [TestCase(0, 200)]
    [TestCase(0, 300)]
    [TestCase(1, 100)]
    [TestCase(1, 200)]
    [TestCase(1, 300)]
    [TestCase(2, 100)]
    [TestCase(2, 200)]
    [TestCase(2, 300)]
    public void ToList_WithPaging_ReturnsAllItems(int page, int pageSize)
    {
        // act
        var results = _cache.TestSet.ToList(page, pageSize);

        // assert
        results.ShouldNotBeEmpty();
        results.Count.ShouldBe(pageSize);
    }

    [TestCase(0, 0)]
    [TestCase(1, 0)]
    [TestCase(2, 0)]
    public void ToList_WithPageSizeZero_ShouldReturnEmptyCollection(int page, int pageSize)
    {
        // act
        var results = _cache.TestSet.ToList(page, pageSize);

        // assert
        results.ShouldBeEmpty();
    }

    [Test]
    public void ToList_WithPaging_PagesShouldBeEquivalent()
    {
        // act
        var results1 = _cache.TestSet.ToList(3, 100);
        var results2 = _cache.TestSet.ToList(3, 100);
        var results3 = _cache.TestSet.ToList(3, 100);

        // assert
        results1.ShouldNotBeEmpty();
        results2.ShouldNotBeEmpty();
        results3.ShouldNotBeEmpty();

        foreach (var item in results1)
        {
            results2.Any(x => x.Id == item.Id).ShouldBeTrue();
            results3.Any(x => x.Id == item.Id).ShouldBeTrue();
        }
    }

    [Test]
    public void ToList_WithPaging_PagesShouldNotBeEquivalent()
    {
        // act
        var results1 = _cache.TestSet.ToList(0, 100);
        var results2 = _cache.TestSet.ToList(1, 100);
        var results3 = _cache.TestSet.ToList(2, 100);

        // assert
        results1.ShouldNotBeEmpty();
        results2.ShouldNotBeEmpty();
        results3.ShouldNotBeEmpty();

        foreach (var item in results1)
        {
            results2.Any(x => x.Id == item.Id).ShouldBeFalse();
            results3.Any(x => x.Id == item.Id).ShouldBeFalse();
        }
    }
}
