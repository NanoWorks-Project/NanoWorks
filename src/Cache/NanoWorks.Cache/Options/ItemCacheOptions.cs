// Ignore Spelling: Nano

using System;
using NanoWorks.Cache.Caches;

namespace NanoWorks.Cache.Options;

/// <summary>
/// Options for a cache containing items of type <typeparamref name="TItem"/>.
/// </summary>
/// <typeparam name="TItem">Type of item in the cache.</typeparam>
public class ItemCacheOptions<TItem>
    where TItem : class, new()
{
    /// <summary>
    /// Gets or sets expiration duration for the cache set.
    /// </summary>
    public TimeSpan ExpirationDuration { get; set; } = TimeSpan.Zero;

    internal Func<TItem, string> KeySelector { get; private set; } = null!;

    internal Type CacheSourceType { get; private set; } = null!;

    /// <summary>
    /// Configures the cache set key selector.
    /// </summary>
    /// <param name="keySelector">Key selector function.</param>
    public void Key(Func<TItem, string> keySelector)
    {
        KeySelector = keySelector;
    }

    /// <summary>
    /// Configures the cache source for the cache set.
    /// </summary>
    /// <typeparam name="TCacheSource">Type of cache source.</typeparam>
    public void Source<TCacheSource>()
        where TCacheSource : class, ICacheSource<TItem>
    {
        CacheSourceType = typeof(TCacheSource);
    }

    /// <summary>
    /// Validates the cache set options.
    /// </summary>
    public virtual void Validate()
    {
        ArgumentNullException.ThrowIfNull(KeySelector, nameof(KeySelector));
        ArgumentNullException.ThrowIfNull(CacheSourceType, nameof(CacheSourceType));

        if (ExpirationDuration <= TimeSpan.Zero)
        {
            throw new InvalidOperationException("Expiration Duration is required");
        }
    }
}
