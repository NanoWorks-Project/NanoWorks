using Microsoft.EntityFrameworkCore;
using Sample.WebApi.Models.Entities;

namespace Sample.WebApi.Data.Database;

/// <summary>
/// Database for the book store.
/// </summary>
public interface IBookStoreDatabase
{
    /// <summary>
    /// Gets the Authors db set.
    /// </summary>
    DbSet<Author> Authors { get; }

    /// <summary>
    /// Gets the Books db set.
    /// </summary>
    DbSet<Book> Books { get; }

    /// <summary>
    /// Saves changes to the database.
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Number of affected entities.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}