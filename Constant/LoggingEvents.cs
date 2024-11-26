using Microsoft.Extensions.Logging;

namespace NetCoreExampleApi.Constant;

public static class LoggingEvents
{
    public static EventId OutboxMessagePublished = new EventId(1001, nameof(OutboxMessagePublished));

    public static EventId OutboxMessagePublishFailed = new EventId(1002, nameof(OutboxMessagePublishFailed));

    public const string StockOrderLogPayload = "{@StockOrderLogPayload}";
}