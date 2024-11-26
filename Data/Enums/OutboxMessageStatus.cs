namespace NetCoreExampleApi.Data.Enums;

public enum OutboxMessageStatus
{
    Unknown = 0,
    New = 1,
    InProgress = 2,
    Published = 3
}