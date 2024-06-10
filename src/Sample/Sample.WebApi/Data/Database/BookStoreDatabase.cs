using Microsoft.EntityFrameworkCore;
using Sample.WebApi.Models.Entities;

namespace Sample.WebApi.Data.Database;

public sealed class BookStoreDatabase(DbContextOptions<BookStoreDatabase> options) : DbContext(options), IBookStoreDatabase
{
    public DbSet<Author> Authors { get; set; } = null!;

    public DbSet<Book> Books { get; set; }
}
