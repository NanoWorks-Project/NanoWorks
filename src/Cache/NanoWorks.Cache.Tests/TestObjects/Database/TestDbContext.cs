// Ignore Spelling: Nano

using Microsoft.EntityFrameworkCore;

namespace NanoWorks.Cache.Tests.TestObjects.Database;

public class TestDbContext : DbContext
{
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Genre> Genres { get; set; } = null!;

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().HasData(
            new Author { Id = Guid.Parse("2e9cf38e-154c-4cac-ac95-1964d56f2478"), FirstName = "John", LastName = "Doe" },
            new Author { Id = Guid.Parse("752cab77-411b-4695-be7f-00a16b4564e3"), FirstName = "Jane", LastName = "Doe" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = Guid.Parse("a51b548d-e0b1-4cba-9c3e-041dffc33464"), Title = "Book 1", AuthorId = Guid.Parse("2e9cf38e-154c-4cac-ac95-1964d56f2478"), GenreId = Guid.Parse("c4f33581-12cc-4776-8808-c66b0d1ec1bb") },
            new Book { Id = Guid.Parse("f3ddb211-fe18-4609-90ee-aa7353f9f98d"), Title = "Book 2", AuthorId = Guid.Parse("752cab77-411b-4695-be7f-00a16b4564e3"), GenreId = Guid.Parse("c4f33581-12cc-4776-8808-c66b0d1ec1bb") },
            new Book { Id = Guid.Parse("e262af69-13da-47d4-82c9-0a13733d8ccb"), Title = "Book 2", AuthorId = Guid.Parse("752cab77-411b-4695-be7f-00a16b4564e3"), GenreId = Guid.Parse("c19eb6ab-57e4-4abf-be4c-b758f04b5f8a") }
        );

        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = Guid.Parse("c4f33581-12cc-4776-8808-c66b0d1ec1bb"), Name = "Fiction" },
            new Genre { Id = Guid.Parse("c19eb6ab-57e4-4abf-be4c-b758f04b5f8a"), Name = "Non-Fiction" }
        );
    }
}
