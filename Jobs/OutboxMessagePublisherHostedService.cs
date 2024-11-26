using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCoreExampleApi.Services.Interfaces;

namespace NetCoreExampleApi.Jobs;

public class OutboxMessagePublisherHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxMessagePublisherHostedService> _logger;
    private static readonly Random Jitter = new Random();
    
    public OutboxMessagePublisherHostedService(
        IServiceProvider serviceProvider, 
        ILogger<OutboxMessagePublisherHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await PublishMessage(stoppingToken);
            
            int delayTime = Jitter.Next(0, 100);
            
            await Task.Delay(delayTime, stoppingToken);
        }
    }

    private async Task PublishMessage(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        {
            IOutboxMessagePublisherService outboxMessagePublisherService = scope.ServiceProvider.GetRequiredService<IOutboxMessagePublisherService>();

            try
            {
                await outboxMessagePublisherService.Publish(stoppingToken);
            }
            catch (SqlException sqlException) when (sqlException.Message.Contains("deadlocked on lock resources with another process"))
            {
                _logger.LogWarning(sqlException, sqlException.Message);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}