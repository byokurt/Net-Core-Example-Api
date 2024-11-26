using System;
using DotnetCoreExampleApi.Data.Entities;

namespace DotnetCoreExampleApi.Factories.Interfaces;

public interface IOutboxMessageFactory
{
    OutboxMessage From<T>(T @event, DateTime now, string queueName = null) where T : class, new();
}