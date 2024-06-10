// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Collections.Generic;
using NanoWorks.Messaging.RabbitMq.ConnectionPools;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.Options
{
    /// <summary>
    /// Options for RabbitMq messaging.
    /// </summary>
    public sealed class MessagingOptions
    {
        /// <summary>
        /// Gets or sets the connection pool.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets the publisher connection from the connection pool.
        /// </summary>
        public IConnection PublisherConnection => ConnectionPool.GetConnection(ConnectionString);

        /// <summary>
        /// Gets the consumer connection from the connection pool.
        /// </summary>
        public IConnection ConsumerConnection => ConnectionPool.GetConnection(ConnectionString);

        /// <summary>
        /// Gets or sets a value indicating whether to use the messaging service.
        /// </summary>
        public bool UseMessagingService { get; set; }

        /// <summary>
        /// Gets or sets the publisher options.
        /// </summary>
        public PublisherOptions PublisherOptions { get; set; } = new PublisherOptions();

        /// <summary>
        /// Gets or sets the consumer options.
        /// </summary>
        public IDictionary<Type, ConsumerOptions> ConsumerOptions { get; set; } = new Dictionary<Type, ConsumerOptions>();

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
            ConnectionPool.Size = connectionPoolSize;
        }

        /// <summary>
        /// Adds a message publisher.
        /// </summary>
        /// <param name="configure">Action to configure the publisher options.</param>
        public void AddMessagePublisher(Action<PublisherOptions> configure)
        {
            configure(PublisherOptions);
        }

        /// <summary>
        /// Adds a message consumer.
        /// </summary>
        /// <typeparam name="TConsumer">Type of consumer.</typeparam>
        /// <param name="configure">Action to configure the consumer options.</param>
        public void AddMessageConsumer<TConsumer>(Action<ConsumerOptions<TConsumer>> configure)
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

            if (ConnectionPool.Size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(ConnectionPool.Size));
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
}
