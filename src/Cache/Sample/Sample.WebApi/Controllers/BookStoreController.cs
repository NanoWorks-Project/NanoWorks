using Microsoft.AspNetCore.Mvc;
using Sample.WebApi.Cache;
using Sample.WebApi.Models;

namespace Sample.WebApi.Controllers;

[ApiController, Route("api")]
public class BookStoreController(IBookStoreCache cache) : ControllerBase
{
    [HttpGet("authors")]
    public IActionResult GetAuthors()
    {
        var authors = cache.Authors;
        return Ok(authors);
    }

    [HttpGet("authors/{authorId}")]
    public IActionResult GetAuthor(Guid authorId)
    {
        var author = cache.Authors[authorId];
        return Ok(author);
    }

    [HttpGet("authors/{authorId}/books")]
    public IActionResult GetAuthorBooks(Guid authorId)
    {
        var authorBooks = cache.AuthorBooks[authorId];

        if (authorBooks?.BookIds == null)
        {
            return Ok(Enumerable.Empty<Book>());
        }

        var books = cache.Books[authorBooks.BookIds];
        return Ok(books);
    }

    [HttpPost("authors")]
    public IActionResult CreateOrUpdateAuthor(Author author)
    {
        var validationErrors = new List<string>();

        if (author == null)
        {
            validationErrors.Add($"{nameof(Author)} is required");
            return BadRequest(validationErrors);
        }

        if (author.AuthorId == default)
        {
            validationErrors.Add($"{nameof(Author)}.{nameof(Author.AuthorId)} is required");
        }

        if (string.IsNullOrWhiteSpace(author.FirstName))
        {
            validationErrors.Add($"{nameof(Author)}.{nameof(Author.FirstName)} is required");
        }

        if (string.IsNullOrWhiteSpace(author.LastName))
        {
            validationErrors.Add($"{nameof(Author)}.{nameof(Author.LastName)} is required");
        }

        if (validationErrors.Count > 0)
        {
            return BadRequest(validationErrors);
        }

        cache.Authors[author.AuthorId] = author;
        return Ok();
    }

    [HttpPost("books")]
    public IActionResult CreateOrUpdateBook(Book book)
    {
        var validationErrors = new List<string>();

        if (book == null)
        {
            validationErrors.Add($"{nameof(Author)} is required");
            return BadRequest(validationErrors);
        }

        var author = cache.Authors[book.AuthorId];

        if (author == null)
        {
            validationErrors.Add($"{nameof(Author)} not found");
            return BadRequest(validationErrors);
        }

        if (string.IsNullOrWhiteSpace(book.Title))
        {
            validationErrors.Add($"{nameof(Book)}.{nameof(Book.Title)} is required");
        }

        if (validationErrors.Count > 0)
        {
            return BadRequest(validationErrors);
        }

        var authorBooks = cache.AuthorBooks[author.AuthorId] ?? new AuthorBooks { AuthorId = author.AuthorId };
        authorBooks.BookIds.Add(book.BookId);

        cache.Books[book.BookId] = book;
        cache.AuthorBooks[authorBooks.AuthorId] = authorBooks;
        return Ok();
    }
}
