﻿// Ignore Spelling: Nano

using NanoWorks.Cache.Redis.CacheSets;
using Sample.WebApi.Models.Dtos;

namespace Sample.WebApi.Data.Cache;

/// <summary>
/// Cache for the book store.
/// </summary>
public interface IBookStoreCache
{
    /// <summary>
    /// Gets Authors cache set.
    /// </summary>
    RedisCacheSet<AuthorBooksDto, Guid> AuthorBooks { get; }

    /// <summary>
    /// Gets Author books cache set.
    /// </summary>
    RedisCacheSet<AuthorDto, Guid> Authors { get; }

    /// <summary>
    /// Gets Books cache set.
    /// </summary>
    RedisCacheSet<BookDto, Guid> Books { get; }
}