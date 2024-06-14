﻿// Ignore Spelling: Nano
// Ignore Spelling: Mq

using System;
using System.Threading.Tasks;
using NanoWorks.Messaging.Errors;
using NanoWorks.Messaging.RabbitMq.Options;
using NanoWorks.Messaging.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NanoWorks.Messaging.RabbitMq.Messages
{
    internal sealed class MessageConsumer : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConsumerOptions _consumerOptions;
        private readonly IModel _channel;

        private EventingBasicConsumer _rabbitMqConsumer;
        private EventingBasicConsumer _rabbitMqRetryConsumer;

        internal MessageConsumer(IServiceProvider serviceProvider, ConsumerOptions consumerOptions, IModel channel)
        {
            _serviceProvider = serviceProvider;
            _consumerOptions = consumerOptions;
            _channel = channel;
        }

        public void Dispose()
        {
            foreach (var consumerTag in _rabbitMqConsumer?.ConsumerTags ?? new string[0])
            {
                _channel.BasicCancel(consumerTag);
            }

            foreach (var consumerTag in _rabbitMqRetryConsumer?.ConsumerTags ?? new string[0])
            {
                _channel.BasicCancel(consumerTag);
            }
        }

        internal void Start()
        {
            _rabbitMqConsumer = new EventingBasicConsumer(_channel);
            _rabbitMqConsumer.Received += async (sender, eventArgs) => await TryProcessMessageAsync(eventArgs);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: _consumerOptions.MaxConcurrency, global: false);
            _channel.BasicConsume(queue: _consumerOptions.QueueName, autoAck: false, consumer: _rabbitMqConsumer);

            _rabbitMqRetryConsumer = new EventingBasicConsumer(_channel);
            _rabbitMqRetryConsumer.Received += async (sender, eventArgs) => await TryProcessMessageAsync(eventArgs);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: _consumerOptions.MaxConcurrency, global: false);
            _channel.BasicConsume(queue: _consumerOptions.RetryQueueName, autoAck: false, consumer: _rabbitMqRetryConsumer);
        }

        private async Task TryProcessMessageAsync(BasicDeliverEventArgs eventArgs)
        {
            var messageProcessor = new MessageProcessor(_serviceProvider, _consumerOptions, _channel, eventArgs);

            try
            {
                await messageProcessor.ProcessMessageAsync();
            }
            catch (Exception error)
            {
                var transportError = new TransportError(_consumerOptions.ConsumerType.FullName, error);
                var transportErrorBody = MessageSerializer.Serialize(transportError, PublisherSerializerExceptionBehavior.Throw);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Type = typeof(TransportError).FullName;

                _channel.BasicPublish(exchange: properties.Type, routingKey: string.Empty, body: transportErrorBody, basicProperties: properties);

                if (_consumerOptions.MaxRetryCount > 0)
                {
                    await messageProcessor.RetryMessageAsync();
                }
            }
        }
    }
}
