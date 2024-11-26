using System;
using System.Collections.Generic;
using System.Linq;
using NetCoreExampleApi.Controllers.V1.Model.Requests.Enums;
using NetCoreExampleApi.Handlers.Interfaces;

namespace NetCoreExampleApi.Handlers;

public class TransactionHandlerResolver : ITransactionHandlerResolver
{
    private readonly IEnumerable<ITransactionHandler> _transactionHandlers;
    
    public TransactionHandlerResolver(IEnumerable<ITransactionHandler> transactionHandlers)
    {
        _transactionHandlers = transactionHandlers;
    }

    public ITransactionHandler GetTransactionHandler(TransactionType type)
    {
        ITransactionHandler transactionHandler = _transactionHandlers.FirstOrDefault(h => h.Type == type);

        if (transactionHandler == null)
        {
            throw new InvalidOperationException($"No handler registered for type {type}");
        }

        return transactionHandler;
    }
}