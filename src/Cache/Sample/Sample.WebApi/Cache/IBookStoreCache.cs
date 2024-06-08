using NanoWorks.Cache.Redis.CacheSets;
using Sample.WebApi.Models;

namespace Sample.WebApi.Cache
{
    public interface IBookStoreCache
    {
        CacheSet<AuthorBooks, Guid> AuthorBooks { get; }
        CacheSet<Author, Guid> Authors { get; }
        CacheSet<Book, Guid> Books { get; }
    }
}