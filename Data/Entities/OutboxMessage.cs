using System;
using DotnetCoreExampleApi.Data.Enums;

namespace DotnetCoreExampleApi.Data.Entities;

public class OutboxMessage
{
    public long Id { get; set; }

    public string Data { get; set; }

    public OutboxMessageStatus Status { get; set; }

    public DateTime OccurredOn { get; set; }


    public DateTime? ProcessedDate { get; set; }

    public string Type { get; set; }

    public string QueueName { get; set; }

    public string RoutingKey { get; set; }
}