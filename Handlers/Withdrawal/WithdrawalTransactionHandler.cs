using System;
using System.Threading;
using System.Threading.Tasks;
using DotnetCoreExampleApi.Controllers.V1.Model.Requests;
using DotnetCoreExampleApi.Controllers.V1.Model.Requests.Enums;
using DotnetCoreExampleApi.Handlers.Interfaces;

namespace DotnetCoreExampleApi.Handlers.Withdrawal;

public class WithdrawalTransactionHandler : ITransactionHandler
{
    public WithdrawalTransactionHandler()
    {
        
    }
    
    public TransactionType Type => TransactionType.Withdrawal;
    
    public async Task<Guid> Execute(CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        Guid transactionId = Guid.NewGuid();
        
        return transactionId;
    }
}