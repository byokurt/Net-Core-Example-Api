using System;
using MassTransit;
using Medallion.Threading.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using NetCoreExampleApi.Consumers;
using NetCoreExampleApi.Jobs;
using StackExchange.Redis;

namespace NetCoreExampleApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        });
    }

    public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        string redisConnectionString = configuration.GetConnectionString("Redis")!;

        services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnectionString; });

        ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);

        RedisDistributedSynchronizationProvider lockProvider = new RedisDistributedSynchronizationProvider(connectionMultiplexer.GetDatabase(), c =>
        {
            c.Expiry(TimeSpan.FromSeconds(10));
            c.BusyWaitSleepTime(TimeSpan.FromMicroseconds(3), TimeSpan.FromMicroseconds(10));
        });

        services.AddSingleton(_ => lockProvider);
    }

    public static void AddBackgroundService(this IServiceCollection services)
    {
        services.AddHostedService<DemoBackgroundService>();
        services.AddHostedService<OutboxMessagePublisherHostedService>();
    }

    public static void AddMasTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<DemoConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(configuration["RabbitMQ:Host"]!), c =>
                {
                    c.Username(configuration["RabbitMQ:UserName"]);
                    c.Password(configuration["RabbitMQ:Password"]);
                });

                cfg.UseConcurrencyLimit(90);

                cfg.UseRetry(configurator =>
                {
                    configurator.Incremental(10, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(2000));
                    configurator.Ignore<MassTransitApplicationException>();
                });

                cfg.ReceiveEndpoint(nameof(DemoConsumer), ep =>
                {
                    ep.ConfigureConsumer<DemoConsumer>(context);
                });
            });
        });
    }
}