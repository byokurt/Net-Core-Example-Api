using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace DotnetCoreExampleApi.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    private const string CorrelationIdHeaderKey = "X-Correlation-ID";

    public async Task Invoke(HttpContext context)
    {
        string correlationId;

        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderKey, out StringValues correlationIds))
        {
            correlationId = correlationIds.FirstOrDefault(k => k == CorrelationIdHeaderKey);
        }
        else
        {
            correlationId = Guid.NewGuid().ToString();

            context.Request.Headers.Append(CorrelationIdHeaderKey, correlationId);
        }

        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.TryGetValue(CorrelationIdHeaderKey, out correlationIds))
            {
                context.Response.Headers.Append(CorrelationIdHeaderKey, correlationId);
            }

            return Task.CompletedTask;
        });

        await _next(context);
    }
}