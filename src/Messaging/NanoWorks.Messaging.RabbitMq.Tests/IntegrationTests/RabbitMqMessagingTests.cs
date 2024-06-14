// Ignore Spelling: Nano
// Ignore Spelling: Mq

using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.DependencyInjection;
using NanoWorks.Messaging.Errors;
using NanoWorks.Messaging.MessagePublishers;
using NanoWorks.Messaging.RabbitMq.DependencyInjection;
using NanoWorks.Messaging.RabbitMq.Options;
using NanoWorks.Messaging.RabbitMq.Services;
using NanoWorks.Messaging.RabbitMq.Tests.TestObjects;
using NanoWorks.Messaging.Serialization;
using Shouldly;

namespace NanoWorks.Messaging.RabbitMq.Tests.IntegrationTests;

/// <summary>
/// /// <summary>
/// These tests require RabbitMq running locally on the default ports.
/// </summary>
/// </summary>
public sealed class RabbitMqMessagingTests : IDisposable
{
    private readonly Fixture _fixture;
    private readonly IServiceScope _serviceScope;
    private readonly IMessagePublisher _messagePublisher;
    private readonly MessagingOptions _messagingOptions;
    private readonly MessagingService _messagingService;

    public RabbitMqMessagingTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization() { ConfigureMembers = true });

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddNanoWorksRabbitMq(options =>
        {
            options.UseConnectionString("amqp://rabbitmq:password@localhost:5672/");
            options.UseConnectionPoolSize(Environment.ProcessorCount);

            options.AddMessagePublisher(publisherOptions =>
            {
                publisherOptions.OnSerializationException(PublisherSerializerExceptionBehavior.Ignore);
            });

            options.AddMessageConsumer<TestMessageConsumer>(consumerOptions =>
            {
                consumerOptions.Queue(nameof(TestMessageConsumer));
                consumerOptions.MaxMessageConcurrency((ushort)Environment.ProcessorCount);
                consumerOptions.MessageTtl(TimeSpan.FromHours(1));
                consumerOptions.Retries(maxRetryCount: 3, retryDelay: TimeSpan.FromMilliseconds(100));
                consumerOptions.OnSerializationException(ConsumerSerializerExceptionBehavior.DeadLetter);
                consumerOptions.AutoDelete();
                consumerOptions.Subscribe<TestSimpleMessage>(consumer => consumer.ReceiveSimpleMessage);
                consumerOptions.Subscribe<TestComplexMessage>(consumer => consumer.ReceiveComplexMessage);
                consumerOptions.Subscribe<TestExceptionMessage>(consumer => consumer.ReceiveExceptionMessage);
                consumerOptions.Subscribe<TransportError>(consumer => consumer.ReceiveTransportError);
            });
        });

        var serviceProvider = serviceCollection.BuildServiceProvider();

        _serviceScope = serviceProvider.CreateScope();
        _messagePublisher = _serviceScope.ServiceProvider.GetRequiredService<IMessagePublisher>();
        _messagingOptions = _serviceScope.ServiceProvider.GetRequiredService<MessagingOptions>();
        _messagingService = new MessagingService(_serviceScope.ServiceProvider, _messagingOptions);
        _messagingService.StartAsync(CancellationToken.None).Wait();
    }

    public void Dispose()
    {
        _messagePublisher.Dispose();
        _messagingService.StopAsync(CancellationToken.None).Wait();
        _serviceScope.Dispose();
        GC.SuppressFinalize(this);
    }

    [Test]
    public async Task Publish_Receive_SimpleMessages()
    {
        // arrange
        var publishedSimpleMessages = _fixture.CreateMany<TestSimpleMessage>();

        // act
        foreach (var publishedSimpleMessage in publishedSimpleMessages)
        {
            await _messagePublisher.PublishAsync(publishedSimpleMessage);
        }

        Thread.Sleep(1000); // wait for messages to be delivered

        // assert
        TestMessageConsumer.SimpleMessages().Count().ShouldBe(publishedSimpleMessages.Count());

        foreach (var publishedSimpleMessage in publishedSimpleMessages)
        {
            var receivedSimpleMessage = TestMessageConsumer.SimpleMessages()
                .SingleOrDefault(x => x.Guid == publishedSimpleMessage.Guid);

            receivedSimpleMessage.ShouldNotBeNull();
            receivedSimpleMessage.Guid.ShouldBe(publishedSimpleMessage.Guid);
            receivedSimpleMessage.String.ShouldBe(publishedSimpleMessage.String);
            receivedSimpleMessage.Integer.ShouldBe(publishedSimpleMessage.Integer);
            receivedSimpleMessage.Float.ShouldBe(publishedSimpleMessage.Float);
            receivedSimpleMessage.DateTime.ShouldBe(publishedSimpleMessage.DateTime);
        }
    }

    [Test]
    public async Task Publish_Receive_ComplexMessages()
    {
        // arrange
        var publishedComplexMessages = _fixture.CreateMany<TestComplexMessage>();

        // act
        foreach (var publishedMessage in publishedComplexMessages)
        {
            await _messagePublisher.PublishAsync(publishedMessage);
        }

        Thread.Sleep(1000); // wait for messages to be delivered

        // assert
        TestMessageConsumer.ComplexMessages().Count().ShouldBe(publishedComplexMessages.Count());

        foreach (var publishedComplexMessage in publishedComplexMessages)
        {
            var receivedComplexMessage = TestMessageConsumer.ComplexMessages()
                .SingleOrDefault(x => x.Guid == publishedComplexMessage.Guid);

            receivedComplexMessage.ShouldNotBeNull();
            receivedComplexMessage.Guid.ShouldBe(publishedComplexMessage.Guid);
            receivedComplexMessage.SimpleMessages.Count().ShouldBe(publishedComplexMessage.SimpleMessages.Count());

            foreach (var publishedSimpleMessage in publishedComplexMessage.SimpleMessages)
            {
                var receivedSimpleMessage = receivedComplexMessage.SimpleMessages
                    .SingleOrDefault(x => x.Guid == publishedSimpleMessage.Guid);

                receivedSimpleMessage.ShouldNotBeNull();
                receivedSimpleMessage.Guid.ShouldBe(publishedSimpleMessage.Guid);
                receivedSimpleMessage.String.ShouldBe(publishedSimpleMessage.String);
                receivedSimpleMessage.Integer.ShouldBe(publishedSimpleMessage.Integer);
                receivedSimpleMessage.Float.ShouldBe(publishedSimpleMessage.Float);
                receivedSimpleMessage.DateTime.ShouldBe(publishedSimpleMessage.DateTime);
            }
        }
    }

    [Test]
    public async Task Consumer_Exception_Thrown_Message_Is_Retried()
    {
        // arrange
        var publishedExceptionMessage = _fixture.Create<TestExceptionMessage>();
        var expectedMessageCount = 3; // three retries

        // act
        await _messagePublisher.PublishAsync(publishedExceptionMessage);

        Thread.Sleep(1000); // wait for messages to be delivered and retried

        // assert
        TestMessageConsumer.ExceptionMessages().Count().ShouldBe(expectedMessageCount);

        foreach (var receivedExceptionMessage in TestMessageConsumer.ExceptionMessages())
        {
            receivedExceptionMessage.ShouldNotBeNull();
            receivedExceptionMessage.Guid.ShouldBe(publishedExceptionMessage.Guid);
        }

        TestMessageConsumer.TransportErrors().Count().ShouldBe(expectedMessageCount);

        foreach (var transportError in TestMessageConsumer.TransportErrors())
        {
            transportError.SubscriberName.ShouldBe(typeof(TestMessageConsumer).FullName);
            transportError.Message.ShouldBe("The method or operation is not implemented.");
            transportError.StackTrace.ShouldNotBeNullOrWhiteSpace();
        }
    }
}
