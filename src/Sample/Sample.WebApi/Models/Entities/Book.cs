namespace Sample.WebApi.Models.Entities;

/// <summary>
/// Book entity.
/// </summary>
public sealed class Book
{
    /// <summary>
    /// Gets or sets the book id.
    /// </summary>
    public Guid BookId { get; set; }

    /// <summary>
    /// Gets or sets the author id.
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the associated author.
    /// </summary>
    public Author Author { get; set; } = null!;
}
