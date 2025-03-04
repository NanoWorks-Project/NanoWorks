﻿// Ignore Spelling: Nano

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NanoWorks.Cache.Abstractions;
using NanoWorks.Messaging.MessagePublishers;
using Sample.WebApi.Data;
using Sample.WebApi.Models.Dtos;
using Sample.WebApi.Models.Entities;
using Sample.WebApi.Models.Events;

namespace Sample.WebApi.Controllers;

/// <summary>
/// Controller for the book store API.
/// </summary>
/// <param name="bookStoreDatabase"><see cref="IBookStoreDatabase"/>.</param>
/// <param name="authorCache"><see cref="ICache{AuthorDto}"/>.</param>
/// <param name="messagePublisher"><see cref="IMessagePublisher"/>.</param>
[ApiController]
[Route("api")]
public sealed class BookStoreController(
    IBookStoreDatabase bookStoreDatabase,
    ICache<AuthorDto> authorCache,
    IMessagePublisher messagePublisher)
    : ControllerBase
{
    /// <summary>
    /// Gets all authors.
    /// </summary>
    [HttpGet("authors")]
    public IActionResult GetAuthors()
    {
        var authors = bookStoreDatabase.Authors.ToList();
        return Ok(authors);
    }

    /// <summary>
    /// Gets an author by Id.
    /// </summary>
    /// <param name="authorId">The author Id.</param>
    [HttpGet("authors/{authorId}")]
    public IActionResult GetAuthor(Guid authorId)
    {
        var author = authorCache[authorId.ToString()];
        return Ok(author);
    }

    /// <summary>
    /// Gets the books for an author.
    /// </summary>
    /// <param name="authorId">The author Id.</param>
    [HttpGet("authors/{authorId}/books")]
    public IActionResult GetAuthorBooks(Guid authorId)
    {
        var author = authorCache[authorId.ToString()];
        return Ok(author?.Books ?? []);
    }

    /// <summary>
    /// Creates an author.
    /// </summary>
    /// <param name="authorDto"><see cref="AuthorDto"/>.</param>
    [HttpPost("authors")]
    public async Task<IActionResult> CreateAuthor(AuthorDto authorDto)
    {
        var validationErrors = new List<string>();

        if (authorDto == null)
        {
            validationErrors.Add($"{nameof(AuthorDto)} is required");
            return BadRequest(validationErrors);
        }

        if (authorDto.AuthorId == default)
        {
            validationErrors.Add($"{nameof(AuthorDto)}.{nameof(AuthorDto.AuthorId)} is required");
        }

        if (string.IsNullOrWhiteSpace(authorDto.FirstName))
        {
            validationErrors.Add($"{nameof(AuthorDto)}.{nameof(AuthorDto.FirstName)} is required");
        }

        if (string.IsNullOrWhiteSpace(authorDto.LastName))
        {
            validationErrors.Add($"{nameof(AuthorDto)}.{nameof(AuthorDto.LastName)} is required");
        }

        if (validationErrors.Count > 0)
        {
            return BadRequest(validationErrors);
        }

        var author = new Author()
        {
            AuthorId = authorDto.AuthorId,
            FirstName = authorDto.FirstName,
            LastName = authorDto.LastName,
        };

        bookStoreDatabase.Authors.Add(author);
        await bookStoreDatabase.SaveChangesAsync();

        var authorUpdatedEvent = new AuthorUpdatedEvent { AuthorId = author.AuthorId };
        await messagePublisher.BroadcastAsync(authorUpdatedEvent);
        return Ok();
    }

    /// <summary>
    /// Updates an author.
    /// </summary>
    /// <param name="authorDto"><see cref="AuthorDto"/>.</param>
    [HttpPut("authors")]
    public async Task<IActionResult> UpdateAuthor(AuthorDto authorDto)
    {
        var validationErrors = new List<string>();

        if (authorDto == null)
        {
            validationErrors.Add($"{nameof(AuthorDto)} is required");
            return BadRequest(validationErrors);
        }

        if (authorDto.AuthorId == default)
        {
            validationErrors.Add($"{nameof(AuthorDto)}.{nameof(AuthorDto.AuthorId)} is required");
        }

        if (string.IsNullOrWhiteSpace(authorDto.FirstName))
        {
            validationErrors.Add($"{nameof(AuthorDto)}.{nameof(AuthorDto.FirstName)} is required");
        }

        if (string.IsNullOrWhiteSpace(authorDto.LastName))
        {
            validationErrors.Add($"{nameof(AuthorDto)}.{nameof(AuthorDto.LastName)} is required");
        }

        if (validationErrors.Count > 0)
        {
            return BadRequest(validationErrors);
        }

        var author = await bookStoreDatabase.Authors
            .SingleOrDefaultAsync(x => x.AuthorId == authorDto.AuthorId);

        if (author == null)
        {
            validationErrors.Add($"{nameof(AuthorDto)} not found");
            return BadRequest(validationErrors);
        }

        author.FirstName = authorDto.FirstName;
        author.LastName = authorDto.LastName;

        bookStoreDatabase.Authors.Update(author);
        await bookStoreDatabase.SaveChangesAsync();

        var authorUpdatedEvent = new AuthorUpdatedEvent { AuthorId = author.AuthorId };
        await messagePublisher.BroadcastAsync(authorUpdatedEvent);
        return Ok();
    }

    /// <summary>
    /// Creates a book.
    /// </summary>
    /// <param name="bookDto"><see cref="BookDto"/>.</param>
    [HttpPost("books")]
    public async Task<IActionResult> CreateBook(BookDto bookDto)
    {
        var validationErrors = new List<string>();

        if (bookDto == null)
        {
            validationErrors.Add($"{nameof(BookDto)} is required");
            return BadRequest(validationErrors);
        }

        var author = await bookStoreDatabase.Authors
            .SingleOrDefaultAsync(x => x.AuthorId == bookDto.AuthorId);

        if (author == null)
        {
            validationErrors.Add($"{nameof(Author)} not found");
            return BadRequest(validationErrors);
        }

        if (string.IsNullOrWhiteSpace(bookDto.Title))
        {
            validationErrors.Add($"{nameof(BookDto)}.{nameof(BookDto.Title)} is required");
        }

        if (validationErrors.Count > 0)
        {
            return BadRequest(validationErrors);
        }

        var book = new Book()
        {
            BookId = bookDto.BookId,
            AuthorId = bookDto.AuthorId,
            Title = bookDto.Title,
        };

        bookStoreDatabase.Books.Add(book);
        await bookStoreDatabase.SaveChangesAsync();

        var bookUpdatedEvent = new BookUpdatedEvent { BookId = book.BookId, AuthorId = book.AuthorId };
        await messagePublisher.BroadcastAsync(bookUpdatedEvent);
        return Ok();
    }

    /// <summary>
    /// Updates a book.
    /// </summary>
    /// <param name="bookDto"><see cref="BookDto"/>.</param>
    [HttpPut("books")]
    public async Task<IActionResult> UpdateBook(BookDto bookDto)
    {
        var validationErrors = new List<string>();

        if (bookDto == null)
        {
            validationErrors.Add($"{nameof(BookDto)} is required");
            return BadRequest(validationErrors);
        }

        var author = await bookStoreDatabase.Authors
            .SingleOrDefaultAsync(x => x.AuthorId == bookDto.AuthorId);

        if (author == null)
        {
            validationErrors.Add($"{nameof(Author)} not found");
            return BadRequest(validationErrors);
        }

        if (string.IsNullOrWhiteSpace(bookDto.Title))
        {
            validationErrors.Add($"{nameof(BookDto)}.{nameof(BookDto.Title)} is required");
        }

        if (validationErrors.Count > 0)
        {
            return BadRequest(validationErrors);
        }

        var book = await bookStoreDatabase.Books
            .SingleOrDefaultAsync(x => x.BookId == bookDto.BookId);

        if (book == null)
        {
            validationErrors.Add($"{nameof(Book)} not found");
            return BadRequest(validationErrors);
        }

        book.Title = bookDto.Title;

        bookStoreDatabase.Books.Update(book);
        await bookStoreDatabase.SaveChangesAsync();

        var bookUpdatedEvent = new BookUpdatedEvent { BookId = book.BookId, AuthorId = book.AuthorId };
        await messagePublisher.BroadcastAsync(bookUpdatedEvent);
        return Ok();
    }
}
