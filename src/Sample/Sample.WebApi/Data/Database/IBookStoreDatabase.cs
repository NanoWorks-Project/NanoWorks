using Microsoft.EntityFrameworkCore;
using Sample.WebApi.Models.Entities;

namespace Sample.WebApi.Data.Database;

public interface IBookStoreDatabase
{
    DbSet<Author> Authors { get; set; }

    DbSet<Book> Books { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}