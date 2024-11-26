using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace NetCoreExampleApi.Extensions;

public static class HttpPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IServiceProvider serviceProvider, HttpRequestMessage request)
    {
        Random jitterer = new Random();

        return HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(20, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)), onRetryAsync: async (outcome, timespan, retryAttempt, context) =>
        {
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            ILogger logger = loggerFactory.CreateLogger("HttpPolicies");

            string httpResponse = null;

            int? httpStatusCode = null;

            if (outcome?.Result != null)
            {
                httpResponse = await outcome.Result.Content.ReadAsStringAsync();
                httpStatusCode = (int) outcome.Result.StatusCode;
            }

            logger.LogWarning(outcome?.Exception, $"Delaying for {timespan.TotalMilliseconds} ms, then making retry {retryAttempt} HttpResponse:{httpResponse} HttpStatusCode:{httpStatusCode}");
        });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(200, TimeSpan.FromSeconds(60));
    }
}