using System;
using System.Threading;
using System.Threading.Tasks;
using DotnetCoreExampleApi.Controllers.V1.Model.Requests;
using DotnetCoreExampleApi.Controllers.V1.Model.Requests.Enums;
using DotnetCoreExampleApi.Handlers.Interfaces;

namespace DotnetCoreExampleApi.Handlers.Deposit;

public class DepositTransactionHandler : ITransactionHandler
{
    public DepositTransactionHandler()
    {
        
    }

    public TransactionType Type => TransactionType.Deposit;
    
    public async Task<Guid> Execute(CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        Guid transactionId = Guid.NewGuid();
        
        return transactionId;
    }
}