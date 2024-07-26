// Ignore Spelling: Nano

using NanoWorks.Cache.Redis.CacheContexts;
using NanoWorks.Cache.Redis.CacheSets;
using NanoWorks.Cache.Redis.Options;
using Sample.WebApi.Models.Dtos;

namespace Sample.WebApi.Data.Cache;

/// <summary>
/// Cache for the book store.
/// </summary>
public sealed class BookStoreCache : RedisCacheContext, IBookStoreCache
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BookStoreCache"/> class.
    /// </summary>
    /// <param name="options"><see cref="RedisCacheContextOptions"/>.</param>
    public BookStoreCache(RedisCacheContextOptions options)
        : base(options)
    {
        Authors = NewSet<AuthorDto, Guid>(options =>
        {
            options.Table("Authors");
            options.Key(author => author.AuthorId);
            options.Expiration(TimeSpan.FromHours(1));
        });

        AuthorBooks = NewSet<AuthorBooksDto, Guid>(options =>
        {
            options.Table("AuthorBooks");
            options.Key(authorBook => authorBook.AuthorId);
            options.Expiration(TimeSpan.FromHours(1));
        });

        Books = NewSet<BookDto, Guid>(options =>
        {
            options.Table("Books");
            options.Key(book => book.BookId);
            options.Expiration(TimeSpan.FromHours(1));
        });
    }

    /// <inheritdoc />
    public RedisCacheSet<AuthorDto, Guid> Authors { get; }

    /// <inheritdoc />
    public RedisCacheSet<AuthorBooksDto, Guid> AuthorBooks { get; }

    /// <inheritdoc />
    public RedisCacheSet<BookDto, Guid> Books { get; }
}
