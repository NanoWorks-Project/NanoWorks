// Ignore Spelling: Nano

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NanoWorks.Actions.Options;

namespace NanoWorks.Actions.DependencyInjection;

/// <summary>
/// Dependency injection extensions for NanoWorks Actions.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds an action to the service collection.
    /// </summary>
    /// <typeparam name="TRequest">Type of request passed to the action.</typeparam>
    /// <typeparam name="TResponse">Type of response returned by the action.</typeparam>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="configure">Action to configure <see cref="ActionOptions{TRequest, TResponse}"/>.</param>
    public static IServiceCollection AddAction<TRequest, TResponse>(
        this IServiceCollection services,
        Action<ActionOptions<TRequest, TResponse>> configure)
        where TRequest : class
        where TResponse : class
    {
        var options = new ActionOptions<TRequest, TResponse>();
        configure(options);

        foreach (var step in options.ProcessingSteps)
        {
            services.AddScoped(step);
        }

        services.AddScoped<IActionScopeProvider, NanoWorksActionScopeProvider>();
        services.AddScoped<IAction<TRequest, TResponse>>(sp =>
        {
            var steps = options.ProcessingSteps.Select(sp.GetRequiredService).Cast<IActionStep<TRequest, TResponse>>();
            var scopeProvider = sp.GetRequiredService<IActionScopeProvider>();
            var logger = sp.GetRequiredService<ILogger<NanoWorksAction>>();
            var action = new NanoWorksAction<TRequest, TResponse>(steps, scopeProvider, logger);
            return action;
        });

        return services;
    }
}
