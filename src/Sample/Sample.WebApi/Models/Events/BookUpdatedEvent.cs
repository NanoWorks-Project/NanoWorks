namespace Sample.WebApi.Models.Events;

/// <summary>
/// Book updated event.
/// </summary>
public sealed class BookUpdatedEvent
{
    /// <summary>
    /// Gets or sets Book id.
    /// </summary>
    public Guid BookId { get; set; }
}
