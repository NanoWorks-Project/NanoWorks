namespace Sample.WebApi.Models;

public class Author
{
    public Guid AuthorId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}
