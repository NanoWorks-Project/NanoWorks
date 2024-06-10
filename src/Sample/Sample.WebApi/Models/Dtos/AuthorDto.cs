using Sample.WebApi.Models.Entities;

namespace Sample.WebApi.Models.Dtos;

public class AuthorDto
{
    public AuthorDto()
    {
    }

    public AuthorDto(Author entity)
    {
        AuthorId = entity.AuthorId;
        FirstName = entity.FirstName;
        LastName = entity.LastName;
    }

    public Guid AuthorId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}
