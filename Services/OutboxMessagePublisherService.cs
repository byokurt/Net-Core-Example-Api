using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MassTransit;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCoreExampleApi.Constant;
using NetCoreExampleApi.Data.Entities;
using NetCoreExampleApi.Services.Interfaces;
using Polly;
using Polly.Fallback;
using Polly.Retry;

namespace NetCoreExampleApi.Services;

public class OutboxMessagePublisherService : IOutboxMessagePublisherService
{
    private readonly IBusControl _busControl;
    private readonly ILogger<OutboxMessagePublisherService> _logger;
    private readonly IConfiguration _configuration;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly RedisDistributedSynchronizationProvider _distributedSynchronizationProvider;

    public OutboxMessagePublisherService(
        IBusControl busControl, 
        ILogger<OutboxMessagePublisherService> logger, 
        IConfiguration configuration, 
        ISendEndpointProvider sendEndpointProvider, 
        RedisDistributedSynchronizationProvider distributedSynchronizationProvider)
    {
        _busControl = busControl;
        _logger = logger;
        _configuration = configuration;
        _sendEndpointProvider = sendEndpointProvider;
        _distributedSynchronizationProvider = distributedSynchronizationProvider;
    }

    public async Task Publish(CancellationToken cancellationToken)
    {
        IEnumerable<OutboxMessage> outboxMessages = await GetOutboxMessages(cancellationToken);

        Parallel.ForEach(outboxMessages, message =>
        {
            FallbackPolicy fallbackPolicy = Policy.Handle<Exception>().Fallback(token =>
            {
                _logger.LogError(LoggingEvents.OutboxMessagePublishFailed, LoggingEvents.StockOrderLogPayload, new
                {
                    MessageId = message.Id,
                    MessageType = message.Type
                });
            });

            RetryPolicy retryPolicy = Policy.Handle<Exception>().WaitAndRetry(5, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount, context) =>
            {
                _logger.LogWarning(LoggingEvents.OutboxMessagePublishFailed, exception, LoggingEvents.StockOrderLogPayload, new
                {
                    RetryCount = retryCount,
                    MessageId = message.Id,
                    MessageType = message.Type
                });
            });

            fallbackPolicy.Wrap(retryPolicy).Execute(() => ProcessMessage(message).Wait());
        });
    }

    private async Task<IEnumerable<OutboxMessage>> GetOutboxMessages(CancellationToken stoppingToken)
    {
        using (await _distributedSynchronizationProvider.AcquireLockAsync("OutboxJobs", cancellationToken: stoppingToken))
        {
            using (var conn = new SqlConnection(_configuration.GetConnectionString("DemoCnn")))
            {
                DateTime utcNow = DateTime.UtcNow;
                DateTime before = DateTime.UtcNow.AddMinutes(-1);

                return await conn.QueryAsync<OutboxMessage>($@"UPDATE TOP(42) [dbo].[OutboxMessages] 
                                                                    SET Status = 2,ProcessedDate=@ProcessedDate
                                                                    OUTPUT Inserted.Id,
                                                                           Inserted.[Type],
                                                                           Inserted.[Data],
                                                                           Inserted.ProcessedDate,
                                                                           Inserted.QueueName,
                                                                           Inserted.RoutingKey
                                                                    WHERE (Status = 1 OR (Status = 2 AND ProcessedDate<=@BeforeProcessedDate))", new { ProcessedDate = utcNow, BeforeProcessedDate = before });
            }
        }
    }

    private async Task ProcessMessage(OutboxMessage outboxMessage)
    {
        await PublishOrSendMessage(outboxMessage);

        await UpdateMessageStatusAsPublished(outboxMessage);
    }

    private async Task PublishOrSendMessage(OutboxMessage outboxMessage)
    {
        var type = Type.GetType(outboxMessage.Type);

        var domainEvent = JsonSerializer.Deserialize(outboxMessage.Data, type);

        if (string.IsNullOrWhiteSpace(outboxMessage.QueueName))
        {
            await Publish(domainEvent, outboxMessage.RoutingKey);
        }
        else
        {
            await Send(domainEvent, outboxMessage.QueueName);
        }
    }

    private async Task Publish(Object domainEvent, string routingKey)
    {
        await _busControl.Publish(domainEvent, p =>
        {
            if (!string.IsNullOrWhiteSpace(routingKey))
            {
                p.SetRoutingKey(routingKey);
            }
        });
    }

    private async Task Send(object domainEvent, string queueName)
    {
        ISendEndpoint endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));

        await endpoint.Send(domainEvent);
    }

    private async Task UpdateMessageStatusAsPublished(OutboxMessage outboxMessage)
    {
        DateTime utcNow = DateTime.UtcNow;

        using (var conn = new SqlConnection(_configuration.GetConnectionString("DemoCnn")))
        {
            await conn.ExecuteAsync(@"UPDATE [dbo].[OutboxMessages] SET Status=3, ProcessedDate=@ProcessedDate WHERE Id=@Id", new { Id = outboxMessage.Id, ProcessedDate = utcNow });
        }
    }
}