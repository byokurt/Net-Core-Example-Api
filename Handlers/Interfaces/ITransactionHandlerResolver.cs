using NetCoreExampleApi.Controllers.V1.Model.Requests.Enums;

namespace NetCoreExampleApi.Handlers.Interfaces;

public interface ITransactionHandlerResolver
{
    ITransactionHandler GetTransactionHandler(TransactionType type);
}