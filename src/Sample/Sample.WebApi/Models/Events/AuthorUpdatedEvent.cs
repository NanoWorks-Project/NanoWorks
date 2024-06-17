namespace Sample.WebApi.Models.Events;

/// <summary>
/// Author updated event.
/// </summary>
public sealed class AuthorUpdatedEvent
{
    /// <summary>
    /// Gets or sets Author id.
    /// </summary>
    public Guid AuthorId { get; set; }
}
