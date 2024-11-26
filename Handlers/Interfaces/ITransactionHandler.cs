using System;
using System.Threading;
using System.Threading.Tasks;
using NetCoreExampleApi.Controllers.V1.Model.Requests;
using NetCoreExampleApi.Controllers.V1.Model.Requests.Enums;

namespace NetCoreExampleApi.Handlers.Interfaces;

public interface ITransactionHandler
{
    TransactionType Type { get; }
    
    Task<Guid> Execute(CreateTransactionRequest request, CancellationToken cancellationToken);
}