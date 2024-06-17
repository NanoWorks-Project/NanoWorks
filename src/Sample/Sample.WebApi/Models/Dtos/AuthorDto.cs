using Sample.WebApi.Models.Entities;

namespace Sample.WebApi.Models.Dtos;

/// <summary>
/// Author DTO.
/// </summary>
public class AuthorDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorDto"/> class.
    /// </summary>
    public AuthorDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorDto"/> class.
    /// </summary>
    /// <param name="entity"><see cref="Author"/>.</param>
    public AuthorDto(Author entity)
    {
        AuthorId = entity.AuthorId;
        FirstName = entity.FirstName;
        LastName = entity.LastName;
    }

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
}
