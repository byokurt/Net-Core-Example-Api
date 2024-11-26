using System;
using System.Threading;
using System.Threading.Tasks;
using NetCoreExampleApi.Controllers.V1.Model.Requests;
using NetCoreExampleApi.Controllers.V1.Model.Requests.Enums;
using NetCoreExampleApi.Handlers.Interfaces;

namespace NetCoreExampleApi.Handlers.Withdrawal;

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