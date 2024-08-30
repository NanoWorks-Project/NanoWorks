namespace Sample.WebApi.Models.Entities;

/// <summary>
/// Author entity.
/// </summary>
public sealed class Author
{
    /// <summary>
    /// Gets or sets the author id.
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the books associated with the author.
    /// </summary>
    public ICollection<Book> Books { get; set; } = [];
}
