using System.Reflection;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;

namespace Rudi.Dev.Azure.Functions.Isolated.Authz;

public static class FunctionContextExtensions
{
    /// <summary>
    /// Get the Claims Principal for this FunctionContext (if any).
    /// </summary>
    /// <param name="context">Function Context</param>
    /// <returns><see cref="ClaimsPrincipal"/></returns>
    public static ClaimsPrincipal? GetClaimsPrincipal(this FunctionContext context)
    {
        if (context.Items.TryGetValue(FunctionsAuthConstants.FunctionsAuthClaimsPrincipalItemName, out var item))
        {
            return item as ClaimsPrincipal;
        }

        return default;
    }

    /// <summary>
    /// Get the Access Token sent with this request (if any).
    /// </summary>
    /// <param name="context">Function Context</param>
    /// <returns>Access Token</returns>
    public static string? GetAccessToken(this FunctionContext context)
    {
        if (context.Items.TryGetValue(FunctionsAuthConstants.FunctionsAuthAccessTokenItemName, out var accessToken))
        {
            return accessToken as string;
        }

        return default;
    }

    internal static List<TAttribute> GetAttributes<TAttribute>(this FunctionContext context)
        where TAttribute : Attribute
    {
        var point = context.FunctionDefinition.EntryPoint.LastIndexOf('.');
        var methodInfo = Assembly.GetExecutingAssembly()
            .GetType(context.FunctionDefinition.EntryPoint[..point])!
            .GetMethod(context.FunctionDefinition.EntryPoint[(point + 1)..])!;

        return methodInfo.GetCustomAttributes<TAttribute>()
            .Concat(methodInfo.DeclaringType!.GetCustomAttributes<TAttribute>())
            .ToList();
    }
}