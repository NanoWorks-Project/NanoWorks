using NanoWorks.Cache.Caches;
using Sample.WebApi.Models.Dtos;
using Sample.WebApi.Models.Entities;
using Sample.WebApi.Models.Events;

namespace Sample.WebApi.Messaging;

/// <summary>
/// Message consumer for updating the cache.
/// </summary>
/// <param name="authorCache"><see cref="ICache{AuthorDto}"/>.</param>
/// <param name="bookCache"><see cref="ICache{BookDto}"/>.</param>
public sealed class CacheConsumer(
    ICache<AuthorDto> authorCache,
    ICache<BookDto> bookCache)
{
    /// <summary>
    /// Updates the cache when an author is updated.
    /// </summary>
    /// <param name="event"><see cref="AuthorUpdatedEvent"/>.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    public async Task OnAuthorUpdated(AuthorUpdatedEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{nameof(Author)} updated '{@event.AuthorId}' - refreshing cache");
        await authorCache.RefreshAsync(@event.AuthorId.ToString(), cancellationToken);
    }

    /// <summary>
    /// Updates the cache when a book is updated.
    /// </summary>
    /// <param name="event"><see cref="BookUpdatedEvent"/>.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    public async Task OnBookUpdated(BookUpdatedEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{nameof(Book)} updated '{@event.BookId}' - refreshing cache");
        await authorCache.RefreshAsync(@event.AuthorId.ToString(), cancellationToken);
        await bookCache.RefreshAsync(@event.BookId.ToString(), cancellationToken);
    }
}
