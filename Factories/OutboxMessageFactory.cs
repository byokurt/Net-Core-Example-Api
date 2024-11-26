using System;
using System.Text.Json;
using DotnetCoreExampleApi.Data.Entities;
using DotnetCoreExampleApi.Data.Enums;
using DotnetCoreExampleApi.Factories.Interfaces;

namespace DotnetCoreExampleApi.Factories;

public class OutboxMessageFactory : IOutboxMessageFactory
{
    public OutboxMessage From<T>(T @event, DateTime now, string queueName = null) where T : class, new()
    {
        string data = JsonSerializer.Serialize(@event);

        string type = @event.GetType().FullName;

        OutboxMessage outboxMessage = new OutboxMessage
        {
            Data = data,
            OccurredOn = now,
            Status = OutboxMessageStatus.New,
            Type = type,
            QueueName = queueName
        };

        return outboxMessage;
    }
}