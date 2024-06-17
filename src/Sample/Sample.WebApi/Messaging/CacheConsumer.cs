using Microsoft.EntityFrameworkCore;
using Sample.WebApi.Data.Cache;
using Sample.WebApi.Data.Database;
using Sample.WebApi.Models.Dtos;
using Sample.WebApi.Models.Entities;
using Sample.WebApi.Models.Events;

namespace Sample.WebApi.Messaging;

/// <summary>
/// Message consumer for updating the cache.
/// </summary>
/// <param name="bookStoreDatabase"><see cref="IBookStoreDatabase"/>.</param>
/// <param name="bookStoreCache"><see cref="IBookStoreCache"/>.</param>
public sealed class CacheConsumer(IBookStoreDatabase bookStoreDatabase, IBookStoreCache bookStoreCache)
{
    /// <summary>
    /// Updates the cache when an author is updated.
    /// </summary>
    /// <param name="event"><see cref="AuthorUpdatedEvent"/>.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    public async Task OnAuthorUpdated(AuthorUpdatedEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{nameof(Author)} updated '{@event.AuthorId}' - syncing with cache");

        var author = await bookStoreDatabase.Authors
            .SingleOrDefaultAsync(x => x.AuthorId == @event.AuthorId, cancellationToken);

        if (author == null)
        {
            return;
        }

        var authorDto = new AuthorDto(author);
        bookStoreCache.Authors[authorDto.AuthorId] = authorDto;
    }

    /// <summary>
    /// Updates the cache when a book is updated.
    /// </summary>
    /// <param name="event"><see cref="BookUpdatedEvent"/>.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    public async Task OnBookUpdated(BookUpdatedEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{nameof(Book)} updated '{@event.BookId}' - syncing with cache");

        var book = await bookStoreDatabase.Books
            .SingleOrDefaultAsync(x => x.BookId == @event.BookId, cancellationToken);

        if (book == null)
        {
            return;
        }

        var bookDto = new BookDto(book);
        bookStoreCache.Books[bookDto.BookId] = bookDto;

        var authorBooks = bookStoreCache.AuthorBooks[bookDto.AuthorId] ?? new AuthorBooksDto(bookDto.AuthorId);
        authorBooks.BookIds.Add(bookDto.BookId);
        bookStoreCache.AuthorBooks[bookDto.AuthorId] = authorBooks;
    }
}
