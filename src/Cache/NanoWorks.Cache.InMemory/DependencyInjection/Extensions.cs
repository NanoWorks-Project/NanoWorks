// Ignore Spelling: Nano

using Microsoft.Extensions.DependencyInjection;
using NanoWorks.Cache.InMemory.CacheContexts;

namespace NanoWorks.Cache.InMemory.DependencyInjection
{
    /// <summary>
    /// Dependency injection extensions for in-memory cache.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Adds NanoWorks In-Memory cache to the service collection.
        /// </summary>
        /// <typeparam name="TCacheContext">Type of <see cref="InMemoryCacheContext"/>.</typeparam>
        /// <param name="services">The service collection.</param>
        public static IServiceCollection AddNanoWorksInMemoryCache<TCacheContext>(this IServiceCollection services)
            where TCacheContext : InMemoryCacheContext
        {
            services.AddScoped<TCacheContext>();
            return services;
        }
    }
}
