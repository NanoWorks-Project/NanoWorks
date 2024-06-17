namespace Sample.WebApi.Models.Dtos;

/// <summary>
/// Author books DTO.
/// </summary>
public class AuthorBooksDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorBooksDto"/> class.
    /// </summary>
    public AuthorBooksDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorBooksDto"/> class.
    /// </summary>
    /// <param name="authorId">The author id.</param>
    public AuthorBooksDto(Guid authorId)
    {
        AuthorId = authorId;
    }

    /// <summary>
    /// Gets or sets the author id.
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the book ids.
    /// </summary>
    public HashSet<Guid> BookIds { get; set; } = [];
}
