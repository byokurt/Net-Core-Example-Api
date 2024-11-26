using System;
using NetCoreExampleApi.Data.Entities;

namespace NetCoreExampleApi.Factories.Interfaces;

public interface IOutboxMessageFactory
{
    OutboxMessage From<T>(T @event, DateTime now, string queueName = null) where T : class, new();
}