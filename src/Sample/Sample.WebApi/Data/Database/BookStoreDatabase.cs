using Microsoft.EntityFrameworkCore;
using Sample.WebApi.Models.Entities;

namespace Sample.WebApi.Data.Database;

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
}
