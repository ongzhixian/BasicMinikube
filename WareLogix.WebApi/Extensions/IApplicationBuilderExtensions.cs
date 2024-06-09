using Microsoft.AspNetCore.Builder;

using WareLogix.WebApi.Middleware;

namespace WareLogix.WebApi.Extensions;

public static class IApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCorrelationIdMiddleware(this IApplicationBuilder applicationBuilder)
    {
        return applicationBuilder.UseMiddleware<CorrelationIdMiddleware>();
    }
}