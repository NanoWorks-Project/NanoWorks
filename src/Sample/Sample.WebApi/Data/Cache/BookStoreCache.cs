// Ignore Spelling: Nano

using NanoWorks.Cache.Options;
using NanoWorks.Cache.Redis.CacheContexts;
using NanoWorks.Cache.Redis.CacheSets;
using NanoWorks.Cache.Redis.Options;
using Sample.WebApi.Models.Dtos;

namespace Sample.WebApi.Data.Cache;

public sealed class BookStoreCache : RedisCacheContext, IBookStoreCache
{
    public BookStoreCache(CacheContextOptions options)
        : base(options)
    {
        Authors = NewSet<AuthorDto, Guid>(options =>
        {
            options.Table("Authors");
            options.Key(author => author.AuthorId);
            options.Expiration(TimeSpan.FromHours(1));
            options.OnSerializationException(SerializerExceptionBehavior.Ignore);
        });

        AuthorBooks = NewSet<AuthorBooksDto, Guid>(options =>
        {
            options.Table("AuthorBooks");
            options.Key(authorBook => authorBook.AuthorId);
            options.Expiration(TimeSpan.FromHours(1));
            options.OnSerializationException(SerializerExceptionBehavior.Ignore);
        });

        Books = NewSet<BookDto, Guid>(options =>
        {
            options.Table("Books");
            options.Key(book => book.BookId);
            options.Expiration(TimeSpan.FromHours(1));
            options.OnSerializationException(SerializerExceptionBehavior.Ignore);
        });
    }

    public CacheSet<AuthorDto, Guid> Authors { get; }

    public CacheSet<AuthorBooksDto, Guid> AuthorBooks { get; }

    public CacheSet<BookDto, Guid> Books { get; }
}
