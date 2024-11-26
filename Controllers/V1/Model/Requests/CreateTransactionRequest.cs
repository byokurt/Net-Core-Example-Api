using DotnetCoreExampleApi.Controllers.V1.Model.Requests.Enums;

namespace DotnetCoreExampleApi.Controllers.V1.Model.Requests;

public class CreateTransactionRequest
{
    public TransactionType Type { get; set; }

    public decimal Amount { get; set; }
}