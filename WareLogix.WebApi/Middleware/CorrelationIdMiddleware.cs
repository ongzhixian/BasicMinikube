using Microsoft.AspNetCore.Http;

using Serilog.Context;

namespace WareLogix.WebApi.Middleware;

public class CorrelationIdMiddleware
{
    private const string correlationIdHeaderKey = "X-Correlation-ID";

    private readonly RequestDelegate next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey(correlationIdHeaderKey))
            context.Request.Headers.Add(correlationIdHeaderKey, Guid.NewGuid().ToString("N"));

        var correlationId = context.Request.Headers[correlationIdHeaderKey].ToString();

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
