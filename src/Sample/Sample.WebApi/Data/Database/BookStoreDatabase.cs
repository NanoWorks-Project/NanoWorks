using Microsoft.EntityFrameworkCore;
using Sample.WebApi.Models.Entities;

namespace Sample.WebApi.Data.Database;

public class BookStoreDatabase : DbContext, IBookStoreDatabase
{
    public BookStoreDatabase(DbContextOptions<BookStoreDatabase> options) 
        : base(options)
    {
    }

    public DbSet<Author> Authors { get; set; } = null!;

    public DbSet<Book> Books { get; set; }
}
