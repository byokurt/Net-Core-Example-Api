using System;
using System.Threading;
using System.Threading.Tasks;
using DotnetCoreExampleApi.Controllers.V1.Model.Requests;
using DotnetCoreExampleApi.Controllers.V1.Model.Requests.Enums;

namespace DotnetCoreExampleApi.Handlers.Interfaces;

public interface ITransactionHandler
{
    TransactionType Type { get; }
    
    Task<Guid> Execute(CreateTransactionRequest request, CancellationToken cancellationToken);
}