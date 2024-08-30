// Ignore Spelling: Nano
// Ignore Spelling: Mq

using Microsoft.EntityFrameworkCore;
using NanoWorks.Cache.DependencyInjection;
using NanoWorks.Messaging.RabbitMq.DependencyInjection;
using NanoWorks.Messaging.Serialization;
using Sample.WebApi.Data;
using Sample.WebApi.Messaging;
using Sample.WebApi.Models.Dtos;
using Sample.WebApi.Models.Events;

namespace Sample.WebApi;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<BookStoreDatabase>(options =>
        {
            options.UseNpgsql("Server=localhost;Port=5432;Database=book_store;User Id=postgres;Password=password;");
        });

        builder.Services.AddScoped<IBookStoreDatabase, BookStoreDatabase>();

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:6379";
        });

        builder.Services.AddNanoWorksCaching(options =>
        {
            options.UseCache<AuthorDto>(cacheOptions =>
            {
                cacheOptions.ExpirationDuration = TimeSpan.FromHours(1);
                cacheOptions.Key(authorDto => authorDto.AuthorId.ToString());
                cacheOptions.Source<BookStoreCacheSource>();
            });

            options.UseCache<BookDto>(cacheOptions =>
            {
                cacheOptions.ExpirationDuration = TimeSpan.FromHours(1);
                cacheOptions.Key(bookDto => bookDto.BookId.ToString());
                cacheOptions.Source<BookStoreCacheSource>();
            });
        });

        builder.Services.AddNanoWorksRabbitMq(options =>
        {
            options.UseConnectionString("amqp://rabbitmq:password@localhost:5672/");

            options.UseMessagePublisher(publisherOptions =>
            {
                publisherOptions.OnSerializationException(PublisherSerializerExceptionBehavior.Ignore);
            });

            options.UseMessageConsumer<CacheConsumer>(consumerOptions =>
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
    }
}