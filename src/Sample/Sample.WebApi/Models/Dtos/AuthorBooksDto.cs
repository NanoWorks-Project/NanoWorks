namespace Sample.WebApi.Models.Dtos;

public class AuthorBooksDto
{
    public AuthorBooksDto()
    {
    }

    public AuthorBooksDto(Guid authorId)
    {
        AuthorId = authorId;
    }

    public Guid AuthorId { get; set; }

    public HashSet<Guid> BookIds { get; set; } = [];
}
