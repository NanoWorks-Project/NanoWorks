namespace Sample.WebApi.Models;

public class Book
{
    public Guid BookId { get; set; }

    public Guid AuthorId { get; set; }

    public string Title { get; set; } = string.Empty;
}
