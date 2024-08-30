using Microsoft.EntityFrameworkCore;
using NanoWorks.Cache.Caches;
using Sample.WebApi.Models.Dtos;

namespace Sample.WebApi.Data
{
    /// <summary>
    /// Cache source for the book store.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="BookStoreCacheSource"/> class.
    /// </remarks>
    /// <param name="database"><see cref="BookStoreDatabase"/>.</param>
    public sealed class BookStoreCacheSource(BookStoreDatabase database) : ICacheSource<AuthorDto>, ICacheSource<BookDto>
    {
        AuthorDto? ICacheSource<AuthorDto>.Get(string key)
        {
            var author = database.Authors
                .Include(author => author.Books)
                .SingleOrDefault(author => author.AuthorId == Guid.Parse(key));

            if (author is null)
            {
                return null;
            }

            var authorDto = new AuthorDto(author);
            return authorDto;
        }

        async Task<AuthorDto?> ICacheSource<AuthorDto>.GetAsync(string key, CancellationToken cancellationToken)
        {
            var author = await database.Authors
                .Include(author => author.Books)
                .SingleOrDefaultAsync(author => author.AuthorId == Guid.Parse(key), cancellationToken);

            if (author is null)
            {
                return null;
            }

            var authorDto = new AuthorDto(author);
            return authorDto;
        }

        BookDto? ICacheSource<BookDto>.Get(string key)
        {
            var book = database.Books
                .SingleOrDefault(book => book.BookId == Guid.Parse(key));

            if (book is null)
            {
                return null;
            }

            var bookDto = new BookDto(book);
            return bookDto;
        }

        async Task<BookDto?> ICacheSource<BookDto>.GetAsync(string key, CancellationToken cancellationToken)
        {
            var book = await database.Books
                .SingleOrDefaultAsync(book => book.BookId == Guid.Parse(key), cancellationToken);

            if (book is null)
            {
                return null;
            }

            var bookDto = new BookDto(book);
            return bookDto;
        }
    }
}
