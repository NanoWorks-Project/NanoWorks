// Ignore Spelling: Nano

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NanoWorks.Cache.Abstractions;
using NanoWorks.Cache.DependencyInjection;
using NanoWorks.Cache.Tests.TestObjects.Cache;
using NanoWorks.Cache.Tests.TestObjects.Database;
using Shouldly;

namespace NanoWorks.Cache.Tests.IntegrationTests;

public class ItemCacheTests : IDisposable
{
    private readonly IServiceScope _scope;
    private readonly TestDbContext _testDbContext;
    private readonly ICache<AuthorSummary> _cache;

    public ItemCacheTests()
    {
        var services = new ServiceCollection();

        services.AddDbContext<TestDbContext>(options => options.UseSqlite("Data Source=ItemCacheTestsDb.dat"));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:6379";
        });

        services.AddNanoWorksCaching(options =>
        {
            options.UseCache<AuthorSummary>(cacheOptions =>
            {
                cacheOptions.ExpirationDuration = TimeSpan.FromSeconds(10);
                cacheOptions.Key(authorSummary => authorSummary.AuthorId.ToString());
                cacheOptions.Source<TestDbContext>(source => source.GetAuthorSummaryAsync);
            });
        });

        var serviceProvider = services.BuildServiceProvider();
        _scope = serviceProvider.CreateScope();
        _cache = _scope.ServiceProvider.GetRequiredService<ICache<AuthorSummary>>();
        _testDbContext = _scope.ServiceProvider.GetRequiredService<TestDbContext>();
        _testDbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _testDbContext.Dispose();
        _scope.Dispose();
        GC.SuppressFinalize(this);
    }

    [Test]
    public void Get_WithIndex_NotFound_ShouldReturnNull()
    {
        // arrange
        var author_Id = Guid.Parse("00000000-0000-0000-0000-000000000000");

        // act
        var authorSummary = _cache[author_Id.ToString()];

        // assert
        authorSummary.ShouldBeNull();
    }

    [Test]
    public void Get_WithIndex_ShouldReturnItem()
    {
        // arrange
        var author_1_Id = Guid.Parse("2e9cf38e-154c-4cac-ac95-1964d56f2478");
        var author_2_Id = Guid.Parse("752cab77-411b-4695-be7f-00a16b4564e3");

        // act
        var authorSummary_1 = _cache[author_1_Id.ToString()];
        var authorSummary_2 = _cache[author_2_Id.ToString()];

        // assert
        authorSummary_1!.AuthorId.ShouldBe(author_1_Id);
        authorSummary_1.AuthorName.ShouldBe("John Doe");
        authorSummary_1.LatestBookTitle.ShouldBe("Book 1");
        authorSummary_1.BookCount.ShouldBe(1);
        authorSummary_1.Genres.Count().ShouldBe(1);
        authorSummary_1.Genres.ShouldContain("Fiction");

        authorSummary_2!.AuthorId.ShouldBe(author_2_Id);
        authorSummary_2.AuthorName.ShouldBe("Jane Doe");
        authorSummary_2.LatestBookTitle.ShouldBe("Book 2");
        authorSummary_2.BookCount.ShouldBe(2);
        authorSummary_2.Genres.Count().ShouldBe(2);
        authorSummary_1.Genres.ShouldContain("Fiction");
        authorSummary_2.Genres.ShouldContain("Non-Fiction");
    }

    [Test]
    public void Get_WithMultipleIndexes_NotFound_ShouldReturnEmpty()
    {
        // arrange
        var authorIds = new List<string>
        {
            Guid.Parse("00000000-0000-0000-0000-000000000000").ToString(),
            Guid.Parse("00000000-0000-0000-0000-000000000001").ToString()
        };

        // act
        var authorSummaries = _cache[authorIds];

        // assert
        authorSummaries.ShouldBeEmpty();
    }

    [Test]
    public void Get_WithMultipleIndexes_ShouldReturnItems()
    {
        // arrange
        var authorIds = new List<string>
        {
            Guid.Parse("2e9cf38e-154c-4cac-ac95-1964d56f2478").ToString(),
            Guid.Parse("752cab77-411b-4695-be7f-00a16b4564e3").ToString()
        };

        // act
        var authorSummaries = _cache[authorIds];

        // assert
        authorSummaries.Count().ShouldBe(2);

        var authorSummary_1 = authorSummaries.First();
        authorSummary_1!.AuthorId.ShouldBe(Guid.Parse("2e9cf38e-154c-4cac-ac95-1964d56f2478"));
        authorSummary_1.AuthorName.ShouldBe("John Doe");
        authorSummary_1.LatestBookTitle.ShouldBe("Book 1");
        authorSummary_1.BookCount.ShouldBe(1);
        authorSummary_1.Genres.Count().ShouldBe(1);
        authorSummary_1.Genres.ShouldContain("Fiction");

        var authorSummary_2 = authorSummaries.Last();
        authorSummary_2!.AuthorId.ShouldBe(Guid.Parse("752cab77-411b-4695-be7f-00a16b4564e3"));
        authorSummary_2.AuthorName.ShouldBe("Jane Doe");
        authorSummary_2.LatestBookTitle.ShouldBe("Book 2");
        authorSummary_2.BookCount.ShouldBe(2);
        authorSummary_2.Genres.Count().ShouldBe(2);
        authorSummary_1.Genres.ShouldContain("Fiction");
        authorSummary_2.Genres.ShouldContain("Non-Fiction");
    }

    [Test]
    public void Get_WithKey_NotFound_ShouldReturnNull()
    {
        // arrange
        var author_Id = "00000000-0000-0000-0000-000000000000";

        // act
        var authorSummary = _cache.Get(author_Id);

        // assert
        authorSummary.ShouldBeNull();
    }

    [Test]
    public void Get_WithKey_ShouldReturnItem()
    {
        // arrange
        var author_1_Id = "2e9cf38e-154c-4cac-ac95-1964d56f2478";
        var author_2_Id = "752cab77-411b-4695-be7f-00a16b4564e3";

        // act
        var authorSummary_1 = _cache.Get(author_1_Id);
        var authorSummary_2 = _cache.Get(author_2_Id);

        // assert
        authorSummary_1!.AuthorId.ShouldBe(Guid.Parse(author_1_Id));
        authorSummary_1.AuthorName.ShouldBe("John Doe");
        authorSummary_1.LatestBookTitle.ShouldBe("Book 1");
        authorSummary_1.BookCount.ShouldBe(1);
        authorSummary_1.Genres.Count().ShouldBe(1);
        authorSummary_1.Genres.ShouldContain("Fiction");

        authorSummary_2!.AuthorId.ShouldBe(Guid.Parse(author_2_Id));
        authorSummary_2.AuthorName.ShouldBe("Jane Doe");
        authorSummary_2.LatestBookTitle.ShouldBe("Book 2");
        authorSummary_2.BookCount.ShouldBe(2);
        authorSummary_2.Genres.Count().ShouldBe(2);
        authorSummary_1.Genres.ShouldContain("Fiction");
        authorSummary_2.Genres.ShouldContain("Non-Fiction");
    }

    [Test]
    public void Get_WithMultipleKeys_NotFound_ShouldReturnEmpty()
    {
        // arrange
        var authorIds = new List<string>
        {
            "00000000-0000-0000-0000-000000000000",
            "00000000-0000-0000-0000-000000000001"
        };

        // act
        var authorSummaries = _cache.Get(authorIds);

        // assert
        authorSummaries.ShouldBeEmpty();
    }

    [Test]
    public void Get_WithMultipleKeys_ShouldReturnItems()
    {
        // arrange
        var authorIds = new List<string>
        {
            "2e9cf38e-154c-4cac-ac95-1964d56f2478",
            "752cab77-411b-4695-be7f-00a16b4564e3"
        };

        // act
        var authorSummaries = _cache.Get(authorIds);

        // assert
        authorSummaries.Count().ShouldBe(2);

        var authorSummary_1 = authorSummaries.First();
        authorSummary_1!.AuthorId.ShouldBe(Guid.Parse("2e9cf38e-154c-4cac-ac95-1964d56f2478"));
        authorSummary_1.AuthorName.ShouldBe("John Doe");
        authorSummary_1.LatestBookTitle.ShouldBe("Book 1");
        authorSummary_1.BookCount.ShouldBe(1);
        authorSummary_1.Genres.Count().ShouldBe(1);
        authorSummary_1.Genres.ShouldContain("Fiction");

        var authorSummary_2 = authorSummaries.Last();
        authorSummary_2!.AuthorId.ShouldBe(Guid.Parse("752cab77-411b-4695-be7f-00a16b4564e3"));
        authorSummary_2.AuthorName.ShouldBe("Jane Doe");
        authorSummary_2.LatestBookTitle.ShouldBe("Book 2");
        authorSummary_2.BookCount.ShouldBe(2);
        authorSummary_2.Genres.Count().ShouldBe(2);
        authorSummary_1.Genres.ShouldContain("Fiction");
        authorSummary_2.Genres.ShouldContain("Non-Fiction");
    }

    [Test]
    public async Task GetAsync_WithKey_NotFound_ShouldReturnNull()
    {
        // arrange
        var author_Id = "00000000-0000-0000-0000-000000000000";

        // act
        var authorSummary = await _cache.GetAsync(author_Id, CancellationToken.None);

        // assert
        authorSummary.ShouldBeNull();
    }

    [Test]
    public async Task GetAsync_WithKey_ShouldReturnItem()
    {
        // arrange
        var author_1_Id = "2e9cf38e-154c-4cac-ac95-1964d56f2478";
        var author_2_Id = "752cab77-411b-4695-be7f-00a16b4564e3";

        // act
        var authorSummary_1 = await _cache.GetAsync(author_1_Id, CancellationToken.None);
        var authorSummary_2 = await _cache.GetAsync(author_2_Id, CancellationToken.None);

        // assert
        authorSummary_1!.AuthorId.ShouldBe(Guid.Parse(author_1_Id));
        authorSummary_1.AuthorName.ShouldBe("John Doe");
        authorSummary_1.LatestBookTitle.ShouldBe("Book 1");
        authorSummary_1.BookCount.ShouldBe(1);
        authorSummary_1.Genres.Count().ShouldBe(1);
        authorSummary_1.Genres.ShouldContain("Fiction");

        authorSummary_2!.AuthorId.ShouldBe(Guid.Parse(author_2_Id));
        authorSummary_2.AuthorName.ShouldBe("Jane Doe");
        authorSummary_2.LatestBookTitle.ShouldBe("Book 2");
        authorSummary_2.BookCount.ShouldBe(2);
        authorSummary_2.Genres.Count().ShouldBe(2);
        authorSummary_1.Genres.ShouldContain("Fiction");
        authorSummary_2.Genres.ShouldContain("Non-Fiction");
    }

    [Test]
    public async Task GetAsync_WithMultipleKeys_NotFound_ShouldReturnEmpty()
    {
        // arrange
        var authorIds = new List<string>
        {
            "00000000-0000-0000-0000-000000000000",
            "00000000-0000-0000-0000-000000000001"
        };

        // act
        var authorSummaries = await _cache.GetAsync(authorIds, CancellationToken.None);

        // assert
        authorSummaries.ShouldBeEmpty();
    }

    [Test]
    public async Task GetAsync_WithMultipleKeys_ShouldReturnItems()
    {
        // arrange
        var authorIds = new List<string>
        {
            "2e9cf38e-154c-4cac-ac95-1964d56f2478",
            "752cab77-411b-4695-be7f-00a16b4564e3"
        };

        // act
        var authorSummaries = await _cache.GetAsync(authorIds, CancellationToken.None);

        // assert
        authorSummaries.Count().ShouldBe(2);

        var authorSummary_1 = authorSummaries.First();
        authorSummary_1!.AuthorId.ShouldBe(Guid.Parse("2e9cf38e-154c-4cac-ac95-1964d56f2478"));
        authorSummary_1.AuthorName.ShouldBe("John Doe");
        authorSummary_1.LatestBookTitle.ShouldBe("Book 1");
        authorSummary_1.BookCount.ShouldBe(1);
        authorSummary_1.Genres.Count().ShouldBe(1);
        authorSummary_1.Genres.ShouldContain("Fiction");

        var authorSummary_2 = authorSummaries.Last();
        authorSummary_2!.AuthorId.ShouldBe(Guid.Parse("752cab77-411b-4695-be7f-00a16b4564e3"));
        authorSummary_2.AuthorName.ShouldBe("Jane Doe");
        authorSummary_2.LatestBookTitle.ShouldBe("Book 2");
        authorSummary_2.BookCount.ShouldBe(2);
        authorSummary_2.Genres.Count().ShouldBe(2);
        authorSummary_1.Genres.ShouldContain("Fiction");
        authorSummary_2.Genres.ShouldContain("Non-Fiction");
    }

    [Test]
    public void Set_ShouldAddItemToCache()
    {
        // arrange
        var author_Id = Guid.NewGuid();

        var authorSummary = new AuthorSummary
        {
            AuthorId = author_Id,
            AuthorName = "John Doe",
            LatestBookTitle = "Book 1",
            BookCount = 1,
            Genres = new List<string> { "Fiction" }
        };

        // act
        _cache.Set(authorSummary);

        // assert
        var cachedAuthorSummary = _cache[author_Id.ToString()];
        cachedAuthorSummary!.ShouldBeEquivalentTo(authorSummary);
    }

    [Test]
    public async Task SetAsync_ShouldAddItemToCache()
    {
        // arrange
        var author_Id = Guid.NewGuid();

        var authorSummary = new AuthorSummary
        {
            AuthorId = author_Id,
            AuthorName = "John Doe",
            LatestBookTitle = "Book 1",
            BookCount = 1,
            Genres = new List<string> { "Fiction" }
        };

        // act
        await _cache.SetAsync(authorSummary, CancellationToken.None);

        // assert
        var cachedAuthorSummary = _cache[author_Id.ToString()];
        cachedAuthorSummary!.ShouldBeEquivalentTo(authorSummary);
    }

    [Test]
    public void Remove_ShouldRemoveItemFromCache()
    {
        // arrange
        var author_Id = Guid.NewGuid();

        var authorSummary = new AuthorSummary
        {
            AuthorId = author_Id,
            AuthorName = "John Doe",
            LatestBookTitle = "Book 1",
            BookCount = 1,
            Genres = new List<string> { "Fiction" }
        };

        _cache.Set(authorSummary);

        // act
        _cache.Remove(author_Id.ToString());

        // assert
        var cachedAuthorSummary = _cache[author_Id.ToString()];
        cachedAuthorSummary.ShouldBeNull();
    }

    [Test]
    public async Task RemoveAsync_ShouldRemoveItemFromCache()
    {
        // arrange
        var author_Id = Guid.NewGuid();

        var authorSummary = new AuthorSummary
        {
            AuthorId = author_Id,
            AuthorName = "John Doe",
            LatestBookTitle = "Book 1",
            BookCount = 1,
            Genres = new List<string> { "Fiction" }
        };

        _cache.Set(authorSummary);

        // act
        await _cache.RemoveAsync(author_Id.ToString(), CancellationToken.None);

        // assert
        var cachedAuthorSummary = _cache[author_Id.ToString()];
        cachedAuthorSummary.ShouldBeNull();
    }

    [Test]
    public void Refresh_ShouldUpdateItemInCacheFromSource()
    {
        // arrange
        var author_Id = Guid.Parse("2e9cf38e-154c-4cac-ac95-1964d56f2478");

        var originalAuthorSummary = _cache[author_Id.ToString()];

        var updatedAuthorSummary = new AuthorSummary
        {
            AuthorId = author_Id,
            AuthorName = "Updated name",
            LatestBookTitle = "Updated Book",
            BookCount = 1,
            Genres = new List<string> { "Updated Genre" }
        };

        _cache.Set(updatedAuthorSummary);

        // act
        _cache.Refresh(author_Id.ToString());

        // assert
        var cachedAuthorSummary = _cache[author_Id.ToString()];
        cachedAuthorSummary!.ShouldBeEquivalentTo(originalAuthorSummary);
    }

    [Test]
    public void Refresh_WithNonExistingKey_ShouldNotAddItemToCache()
    {
        // arrange
        var author_Id = Guid.NewGuid();

        // act
        _cache.Refresh(author_Id.ToString());

        // assert
        var cachedAuthorSummary = _cache[author_Id.ToString()];
        cachedAuthorSummary.ShouldBeNull();
    }

    [Test]
    public async Task RefreshAsync_ShouldUpdateItemInCacheFromSource()
    {
        // arrange
        var author_Id = Guid.Parse("2e9cf38e-154c-4cac-ac95-1964d56f2478");

        var originalAuthorSummary = _cache[author_Id.ToString()];

        var updatedAuthorSummary = new AuthorSummary
        {
            AuthorId = author_Id,
            AuthorName = "Updated name",
            LatestBookTitle = "Updated Book",
            BookCount = 1,
            Genres = new List<string> { "Updated Genre" }
        };

        _cache.Set(updatedAuthorSummary);

        // act
        await _cache.RefreshAsync(author_Id.ToString(), CancellationToken.None);

        // assert
        var cachedAuthorSummary = _cache[author_Id.ToString()];
        cachedAuthorSummary!.ShouldBeEquivalentTo(originalAuthorSummary);
    }

    [Test]
    public async Task RefreshAsync_WithNonExistingKey_ShouldNotAddItemToCache()
    {
        // arrange
        var author_Id = Guid.NewGuid();

        // act
        await _cache.RefreshAsync(author_Id.ToString(), CancellationToken.None);

        // assert
        var cachedAuthorSummary = _cache[author_Id.ToString()];
        cachedAuthorSummary.ShouldBeNull();
    }
}
