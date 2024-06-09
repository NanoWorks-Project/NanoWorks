using NanoWorks.Cache.Redis.CacheSets;
using Sample.WebApi.Models.Dtos;

namespace Sample.WebApi.Data.Cache;

public interface IBookStoreCache
{
    CacheSet<AuthorBooksDto, Guid> AuthorBooks { get; }
    CacheSet<AuthorDto, Guid> Authors { get; }
    CacheSet<BookDto, Guid> Books { get; }
}