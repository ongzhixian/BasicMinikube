namespace SimpleWebApi.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeaderKey = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey(CorrelationIdHeaderKey))
            context.Request.Headers.Append(CorrelationIdHeaderKey, Guid.NewGuid().ToString("N"));

        var correlationId = context.Request.Headers[CorrelationIdHeaderKey];

        //
        //using (LogContext.PushProperty("CorrelationId", correlationId))
        //{
        //    await next(context);
        //}

        await next(context);
    }
}
