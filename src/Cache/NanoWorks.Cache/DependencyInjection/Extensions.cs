// Ignore Spelling: Nano

using System;
using Microsoft.Extensions.DependencyInjection;
using NanoWorks.Cache.Options;

namespace NanoWorks.Cache.DependencyInjection;

/// <summary>
/// Dependency injection extensions for NanoWorks caching.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds NanoWorks caching to the service collection.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="configure">Action to configure <see cref="CacheOptions"/>.</param>
    public static void AddNanoWorksCaching(
        this IServiceCollection services, Action<CacheOptions> configure)
    {
        var cacheOptions = new CacheOptions(services);
        configure(cacheOptions);
    }
}
