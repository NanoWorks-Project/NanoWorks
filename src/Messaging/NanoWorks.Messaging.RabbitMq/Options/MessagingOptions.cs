// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Collections.Generic;
using NanoWorks.Messaging.RabbitMq.ConnectionPools;

namespace NanoWorks.Messaging.RabbitMq.Options;

/// <summary>
/// Options for RabbitMq messaging.
/// </summary>
public sealed class MessagingOptions
{
    internal string ConnectionString { get; set; }

    internal int ConnectionPoolSize { get; set; } = Environment.ProcessorCount;

    internal PublisherOptions PublisherOptions { get; set; } = new PublisherOptions();

    internal IDictionary<Type, ConsumerOptions> ConsumerOptions { get; set; } = new Dictionary<Type, ConsumerOptions>();

    /// <summary>
    /// Sets the connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    public void UseConnectionString(string connectionString)
    {
        ConnectionString = connectionString;
    }

    /// <summary>
    /// Sets the connection pool size.
    /// </summary>
    /// <param name="connectionPoolSize">Maximum number of connections.</param>
    public void UseConnectionPoolSize(int connectionPoolSize)
    {
        ConnectionPoolSize = connectionPoolSize;
    }

    /// <summary>
    /// Adds a message publisher to the service collection.
    /// </summary>
    /// <param name="configure">Action to configure the publisher options.</param>
    public void ConfigureMessagePublisher(Action<PublisherOptions> configure)
    {
        configure(PublisherOptions);
    }

    /// <summary>
    /// Adds a message consumer to the service collection.
    /// </summary>
    /// <typeparam name="TConsumer">Type of consumer.</typeparam>
    /// <param name="configure">Action to configure the consumer options.</param>
    public void ConfigureMessageConsumer<TConsumer>(Action<ConsumerOptions<TConsumer>> configure)
        where TConsumer : class
    {
        var options = new ConsumerOptions<TConsumer>();
        configure(options);

        if (ConsumerOptions.ContainsKey(typeof(TConsumer)))
        {
            ConsumerOptions[typeof(TConsumer)] = options;
            throw new Exception($"Consumer of type {typeof(TConsumer).FullName} already exists.");
        }

        ConsumerOptions[typeof(TConsumer)] = options;
    }

    /// <summary>
    /// Validates the messaging options.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            throw new ArgumentNullException(nameof(ConnectionString));
        }

        if (PublisherOptions == null)
        {
            throw new ArgumentNullException(nameof(PublisherOptions));
        }

        if (ConnectionPoolSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(ConnectionPoolSize));
        }

        PublisherOptions.Validate();

        foreach (var consumerOptions in ConsumerOptions.Values)
        {
            if (consumerOptions == null)
            {
                throw new ArgumentNullException(nameof(consumerOptions));
            }

            consumerOptions.Validate();
        }
    }
}
