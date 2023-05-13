namespace Rudi.Dev.Azure.Functions.Isolated.Authz;

/// <summary>
/// Require that this Function is authenticated and/or authorized.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute
{
    public string[] Roles { get; } = Array.Empty<string>();
    
    public AuthorizeAttribute() {}

    public AuthorizeAttribute(string role)
    {
        Roles = new[] { role };
    }

    public AuthorizeAttribute(string[] roles)
    {
        Roles = roles;
    }
}