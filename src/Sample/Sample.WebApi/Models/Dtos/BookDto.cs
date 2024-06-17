using Sample.WebApi.Models.Entities;

namespace Sample.WebApi.Models.Dtos;

/// <summary>
/// Book DTO.
/// </summary>
public class BookDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BookDto"/> class.
    /// </summary>
    public BookDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BookDto"/> class.
    /// </summary>
    /// <param name="entity"><see cref="Book"/>.</param>
    public BookDto(Book entity)
    {
        BookId = entity.BookId;
        AuthorId = entity.AuthorId;
        Title = entity.Title;
    }

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
}
