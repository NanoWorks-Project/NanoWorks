// Ignore Spelling: Nano Utc

namespace NanoWorks.Cache.Tests.TestObjects.Database;

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime PublishedUtc { get; set; }
    public Guid AuthorId { get; set; }
    public Author? Author { get; set; }
    public Guid GenreId { get; set; }
    public Genre? Genre { get; set; }
}
