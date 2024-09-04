using Microsoft.EntityFrameworkCore;
using Sample.WebApi.Models.Dtos;
using Sample.WebApi.Models.Entities;

namespace Sample.WebApi.Data;

/// <summary>
/// Database for the book store.
/// </summary>
/// <param name="options"><see cref="DbContextOptions"/>.</param>
public sealed class BookStoreDatabase(DbContextOptions<BookStoreDatabase> options) : DbContext(options), IBookStoreDatabase
{
    /// <inheritdoc />
    public DbSet<Author> Authors { get; set; } = null!;

    /// <inheritdoc />
    public DbSet<Book> Books { get; set; }

    /// <summary>
    /// Gets an author by the specified key or null if not found.
    /// </summary>
    /// <param name="key">Author key.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    public async Task<AuthorDto?> GetAuthorAsync(string key, CancellationToken cancellationToken)
    {
        var author = await Authors
            .Include(author => author.Books)
            .SingleOrDefaultAsync(author => author.AuthorId == Guid.Parse(key), cancellationToken);

        if (author is null)
        {
            return null;
        }

        var authorDto = new AuthorDto(author);
        return authorDto;
    }

    /// <summary>
    /// Gets a books by the specified key or null if not found.
    /// </summary>
    /// <param name="key">Book key.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    public async Task<BookDto?> GetBookAsync(string key, CancellationToken cancellationToken)
    {
        var book = await Books
            .SingleOrDefaultAsync(book => book.BookId == Guid.Parse(key), cancellationToken);

        if (book is null)
        {
            return null;
        }

        var bookDto = new BookDto(book);
        return bookDto;
    }
}
