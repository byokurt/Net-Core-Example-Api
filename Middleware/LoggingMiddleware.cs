using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace NetCoreExampleApi.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        using (LogContext.PushProperty("RequestQueryString", httpContext.Request.QueryString.ToString()))
        using (LogContext.PushProperty("RequestScheme", httpContext.Request.Scheme))
        using (LogContext.PushProperty("RequestMethod", httpContext.Request.Method))
        using (LogContext.PushProperty("RequestClientIp", httpContext.Connection.RemoteIpAddress))
        using (LogContext.PushProperty("RequestHost", httpContext.Request.Host))
        using (LogContext.PushProperty("RequestUserAgent", httpContext.Request.Headers["User-Agent"].ToString()))
        {
            await _next(httpContext);
        }
    }
}