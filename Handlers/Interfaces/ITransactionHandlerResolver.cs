using DotnetCoreExampleApi.Controllers.V1.Model.Requests.Enums;

namespace DotnetCoreExampleApi.Handlers.Interfaces;

public interface ITransactionHandlerResolver
{
    ITransactionHandler GetTransactionHandler(TransactionType type);
}