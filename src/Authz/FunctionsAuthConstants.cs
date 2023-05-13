namespace Rudi.Dev.Azure.Functions.Isolated.Authz;

public static class FunctionsAuthConstants
{
    /// <summary>
    /// The Item name in the FunctionContext for the Claims Principal.
    /// </summary>
    public const string FunctionsAuthClaimsPrincipalItemName = "FA_Principal";
    
    /// <summary>
    /// The Item name in the FunctionContext for the Access Token.
    /// </summary>
    public const string FunctionsAuthAccessTokenItemName = "FA_AccessToken";

    public const string AadAccessTokenHeaderName = "X-MS-TOKEN-AAD-ACCESS-TOKEN";
}