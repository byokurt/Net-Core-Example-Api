using NetCoreExampleApi.Controllers.V1.Model.Requests.Enums;

namespace NetCoreExampleApi.Controllers.V1.Model.Requests;

public class CreateTransactionRequest
{
    public TransactionType Type { get; set; }

    public decimal Amount { get; set; }
}