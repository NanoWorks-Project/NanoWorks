// Ignore Spelling: Nano

using NanoWorks.Cache.Tests.TestObjects.Database;

namespace NanoWorks.Cache.Tests.TestObjects.Cache;

public class AuthorSummary
{
    public AuthorSummary()
    {
    }

    public AuthorSummary(Author author)
    {
        AuthorId = author.Id;
        AuthorName = $"{author.FirstName} {author.LastName}";
        LatestBookTitle = author.Books.OrderByDescending(b => b.PublishedUtc).FirstOrDefault()?.Title ?? string.Empty;
        BookCount = author.Books.Count;
        Genres = author.Books.Select(b => b.Genre?.Name ?? string.Empty).Distinct();
    }

    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
    public string LatestBookTitle { get; set; } = null!;
    public int BookCount { get; set; }
    public IEnumerable<string> Genres { get; set; } = new List<string>();
}
