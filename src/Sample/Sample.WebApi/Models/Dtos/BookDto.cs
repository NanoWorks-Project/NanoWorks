using Sample.WebApi.Models.Entities;

namespace Sample.WebApi.Models.Dtos;

public class BookDto
{
    public BookDto()
    {
    }

    public BookDto(Book entity)
    {
        BookId = entity.BookId;
        AuthorId = entity.AuthorId;
        Title = entity.Title;
    }

    public Guid BookId { get; set; }

    public Guid AuthorId { get; set; }

    public string Title { get; set; } = string.Empty;
}
