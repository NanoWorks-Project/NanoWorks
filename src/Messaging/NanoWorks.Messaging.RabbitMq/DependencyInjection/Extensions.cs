// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NanoWorks.Messaging.MessagePublishers;
using NanoWorks.Messaging.RabbitMq.Messages;
using NanoWorks.Messaging.RabbitMq.Options;
using NanoWorks.Messaging.RabbitMq.Services;

namespace NanoWorks.Messaging.RabbitMq.DependencyInjection;

/// <summary>
/// Dependency injection extensions for RabbitMq messaging.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds NanoWorks RabbitMq messaging to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure <see cref="MessagingOptions"/>.</param>
    public static void AddNanoWorksRabbitMq(this IServiceCollection services, Action<MessagingOptions> configure)
    {
        var options = new MessagingOptions();
        configure(options);
        options.Validate();
        services.AddSingleton(options);
        services.AddSingleton<IMessagePublisher, MessagePublisher>();

        foreach (var consumerOptions in options.ConsumerOptions.Values)
        {
            services.AddScoped(consumerOptions.ConsumerType);
        }

        if (options.ConsumerOptions.Any())
        {
            services.AddHostedService<MessagingService>();
        }
    }
}
