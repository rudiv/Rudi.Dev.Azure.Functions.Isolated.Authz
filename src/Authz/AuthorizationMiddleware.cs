using System.Net;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Rudi.Dev.Azure.Functions.Isolated.Authz;

public class AuthorizationMiddleware: IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        // Only run this on HTTP triggers
        var httpRequestData = await context.GetHttpRequestDataAsync();
        if (httpRequestData != null)
        {
            // Check if we have an attribute on the target func
            bool authorized = true;
            ClaimsPrincipal? principal;
            var attributes = context.GetAttributes<AuthorizeAttribute>();
            // If we have an attribute, we at least need a Principal
            if (attributes.Any())
            {
                if ((principal = context.GetClaimsPrincipal()) == null)
                {
                    authorized = false;
                }
                else
                {
                    // Check for any required roles within the attributes
                    var rolesRequired = !attributes.Any() ? new List<string>() : attributes.SelectMany(m => m.Roles).Distinct().ToList();
                    if (rolesRequired.Any(role => !principal.IsInRole(role)))
                    {
                        authorized = false;
                    }
                }
            }

            if (!authorized)
            {
                var responseData = context.GetHttpResponseData();
                if (responseData == null)
                {
                    
                    responseData = httpRequestData.CreateResponse(HttpStatusCode.Unauthorized);
                }
                else
                {
                    responseData.StatusCode = HttpStatusCode.Unauthorized;
                }

                await responseData.WriteStringAsync(nameof(HttpStatusCode.Unauthorized));

                // If we have multiple output bindings, find the HTTP one
                var httpOutputBinding = context.GetOutputBindings<HttpResponseData>().FirstOrDefault(b => b.BindingType == "http" && b.Name != "$return");
                if (httpOutputBinding != null)
                {
                    httpOutputBinding.Value = responseData;
                }
                else
                {
                    context.GetInvocationResult().Value = responseData;
                }

                return;
            }
        }

        await next(context);
    }

    private List<string>? GetRoles(FunctionContext context)
    {
        var attributes = context.GetAttributes<AuthorizeAttribute>();   
        return !attributes.Any() ? default : attributes.SelectMany(m => m.Roles).Distinct().ToList();
    }
}