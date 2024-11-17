using SimpleWebApi.Middleware;

namespace SimpleWebApi.Extensions;

public static class IApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCorrelationIdMiddleware(this IApplicationBuilder applicationBuilder)
    {
        return applicationBuilder.UseMiddleware<CorrelationIdMiddleware>();
    }
}
