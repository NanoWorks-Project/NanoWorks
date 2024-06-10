namespace Sample.WebApi.Models.Events;

public sealed class BookUpdatedEvent
{
    public Guid BookId { get; set; }
}
