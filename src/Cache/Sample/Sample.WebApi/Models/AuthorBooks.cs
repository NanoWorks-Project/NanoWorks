namespace Sample.WebApi.Models;

public class AuthorBooks
{
    public Guid AuthorId { get; set; }
    public HashSet<Guid> BookIds { get; set; } = [];
}
