using System;
using System.Collections.Generic;
using NanoWorks.Messaging.RabbitMq.ConnectionPools;
using RabbitMQ.Client;

namespace NanoWorks.Messaging.RabbitMq.Options
{
    /// <summary>
    /// Options for RabbitMq messaging.
    /// </summary>
    public class MessagingOptions
    {
        internal MessagingOptions()
        {
        }

        internal string ConnectionString { get; set; }

        internal IConnection PublisherConnection => ConnectionPool.GetConnection(ConnectionString);

        internal IConnection ConsumerConnection => ConnectionPool.GetConnection(ConnectionString);

        internal bool UseMessagingService { get; set; }

        internal PublisherOptions PublisherOptions { get; set; } = new PublisherOptions();

        internal IDictionary<Type, ConsumerOptions> ConsumerOptions { get; } = new Dictionary<Type, ConsumerOptions>();

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

        internal void Validate()
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
