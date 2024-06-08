using NanoWorks.Cache.Options;
using NanoWorks.Cache.Redis.CacheContexts;
using NanoWorks.Cache.Redis.CacheSets;
using NanoWorks.Cache.Redis.Options;
using Sample.WebApi.Models;

namespace Sample.WebApi.Cache;

public class BookStoreCache : RedisCacheContext, IBookStoreCache
{
    public BookStoreCache(CacheContextOptions options) : base(options)
    {
        Authors = NewSet<Author, Guid>(options =>
        {
            options.Table("Authors");
            options.Key(author => author.AuthorId);
            options.Expiration(TimeSpan.FromHours(1));
            options.OnSerializationException(SerializerExceptionBehavior.Ignore);
        });

        AuthorBooks = NewSet<AuthorBooks, Guid>(options =>
        {
            options.Table("AuthorBooks");
            options.Key(authorBook => authorBook.AuthorId);
            options.Expiration(TimeSpan.FromHours(1));
            options.OnSerializationException(SerializerExceptionBehavior.Ignore);
        });

        Books = NewSet<Book, Guid>(options =>
        {
            options.Table("Books");
            options.Key(book => book.BookId);
            options.Expiration(TimeSpan.FromHours(1));
            options.OnSerializationException(SerializerExceptionBehavior.Ignore);
        });
    }

    public CacheSet<Author, Guid> Authors { get; }

    public CacheSet<AuthorBooks, Guid> AuthorBooks { get; }

    public CacheSet<Book, Guid> Books { get; }
}
