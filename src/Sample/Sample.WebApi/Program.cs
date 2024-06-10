// Ignore Spelling: Nano
// Ignore Spelling: Mq

using Microsoft.EntityFrameworkCore;
using NanoWorks.Cache.Redis.DependencyInjection;
using NanoWorks.Messaging.RabbitMq.DependencyInjection;
using NanoWorks.Messaging.Serialization;
using Sample.WebApi.Data.Cache;
using Sample.WebApi.Data.Database;
using Sample.WebApi.Messaging;
using Sample.WebApi.Models.Events;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BookStoreDatabase>(options =>
{
    options.UseNpgsql("Server=localhost;Port=5432;Database=book_store;User Id=postgres;Password=password;");
});

builder.Services.AddScoped<IBookStoreDatabase, BookStoreDatabase>();

builder.Services.AddNanoWorksRedisCache<BookStoreCache>(options =>
{
    options.UseConnectionString("localhost:6379");
    options.UseConnectionPoolSize(Environment.ProcessorCount);
});

builder.Services.AddScoped<IBookStoreCache, BookStoreCache>();

builder.Services.AddNanoWorksRabbitMq(options =>
{
    options.UseConnectionString("amqp://rabbitmq:password@localhost:5672/");

    options.AddMessagePublisher(publisherOptions =>
    {
        publisherOptions.OnSerializationException(PublisherSerializerExceptionBehavior.Ignore);
    });

    options.AddMessageConsumer<CacheConsumer>(consumerOptions =>
    {
        consumerOptions.Queue(nameof(CacheConsumer));
        consumerOptions.MaxMessageConcurrency(10);
        consumerOptions.MessageTtl(TimeSpan.FromHours(1));
        consumerOptions.Retries(maxRetryCount: 3, retryDelay: TimeSpan.FromSeconds(1));
        consumerOptions.AutoDelete();
        consumerOptions.OnSerializationException(ConsumerSerializerExceptionBehavior.DeadLetter);

        consumerOptions.Subscribe<AuthorUpdatedEvent>(consumer => consumer.OnAuthorUpdated);
        consumerOptions.Subscribe<BookUpdatedEvent>(consumer => consumer.OnBookUpdated);
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookStoreDatabase>();
    await context.Database.MigrateAsync();
    await context.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
