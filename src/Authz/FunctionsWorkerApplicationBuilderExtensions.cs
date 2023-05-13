using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;

namespace Rudi.Dev.Azure.Functions.Isolated.Authz;

public static class FunctionsWorkerApplicationBuilderExtensions
{
    public static IFunctionsWorkerApplicationBuilder AddAuthz(this IFunctionsWorkerApplicationBuilder builder)
    {
        builder.UseMiddleware<AuthenticationMiddleware>();
        builder.UseMiddleware<AuthorizationMiddleware>();
        return builder;
    }
}