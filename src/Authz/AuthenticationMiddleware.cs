using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Rudi.Dev.Azure.Functions.Isolated.Authz;

public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        // Only run this on HTTP triggers
        var httpRequestData = await context.GetHttpRequestDataAsync();
        if (httpRequestData != null)
        {
            // Access Token
            if (httpRequestData.Headers.TryGetValues(FunctionsAuthConstants.AadAccessTokenHeaderName, out var accessToken))
            {
                context.Items.Add(FunctionsAuthConstants.FunctionsAuthAccessTokenItemName, accessToken.First());
            }
            
            // ClaimsPrincipal
            var claimsPrincipal = ClientPrincipal.ClaimsPrincipalFromRequestData(httpRequestData);
            if (claimsPrincipal != null)
            {
                context.Items.Add(FunctionsAuthConstants.FunctionsAuthClaimsPrincipalItemName, claimsPrincipal);
            }
        }

        await next(context);
    }
}