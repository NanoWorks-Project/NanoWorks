﻿// Ignore Spelling: Nano

using System;
using Microsoft.Extensions.DependencyInjection;
using NanoWorks.Cache.Redis.CacheContexts;
using NanoWorks.Cache.Redis.Options;

namespace NanoWorks.Cache.Redis.DependencyInjection;

/// <summary>
/// Dependency injection extensions for Redis cache.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds NanoWorks Redis cache to the service collection.
    /// </summary>
    /// <typeparam name="TCacheContext">Type of <see cref="RedisCacheContext"/>.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure <see cref="RedisCacheContextOptions"/>.</param>
    public static IServiceCollection AddNanoWorksRedisCache<TCacheContext>(this IServiceCollection services, Action<RedisCashSetOptions<TCacheContext>> configure)
        where TCacheContext : RedisCacheContext
    {
        var options = new RedisCashSetOptions<TCacheContext>();
        configure(options);
        options.Validate();
        services.AddSingleton(options as RedisCacheContextOptions);
        services.AddScoped<TCacheContext>();
        return services;
    }
}
