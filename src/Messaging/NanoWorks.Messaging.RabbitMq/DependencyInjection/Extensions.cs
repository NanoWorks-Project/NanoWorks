﻿// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NanoWorks.Messaging.MessagePublishers;
using NanoWorks.Messaging.RabbitMq.ConnectionPools;
using NanoWorks.Messaging.RabbitMq.Messaging;
using NanoWorks.Messaging.RabbitMq.Options;
using NanoWorks.Messaging.RabbitMq.Services;
using NanoWorks.Messaging.RabbitMq.TransportErrors;

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
        services.AddSingleton<IConnectionPool, ConnectionPool>();
        services.AddSingleton<IMessageSerializer, MessageSerializer>();
        services.AddSingleton<ITransportErrorPublisher, TransportErrorPublisher>();
        services.AddSingleton<IMessageRetry, MessageRetry>();
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
