// Ignore Spelling: Nano
// Ignore Spelling: Utc

namespace NanoWorks.Cache.Redis.Tests.TestObjects;

/// <summary>
/// Test item for the cache.
/// </summary>
public sealed class CacheTestItem
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Gets or sets the created date.
    /// </summary>
    public DateTime CreatedUtc { get; set; }
}
