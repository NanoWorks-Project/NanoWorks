namespace Sample.WebApi.Models.Entities;

public sealed class Author
{
    public Guid AuthorId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}
