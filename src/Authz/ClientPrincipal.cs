using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker.Http;

namespace Rudi.Dev.Azure.Functions.Isolated.Authz;

public class ClientPrincipalClaim
{
    [JsonPropertyName("typ")] public string Type { get; set; } = default!;
    [JsonPropertyName("val")] public string Value { get; set; } = default!;
}

public class ClientPrincipal
{
    [JsonPropertyName("auth_typ")] public string IdentityProvider { get; set; } = default!;
    [JsonPropertyName("name_typ")] public string NameClaimType { get; set; } = ClaimTypes.Name;
    [JsonPropertyName("role_typ")] public string RoleClaimType { get; set; } = ClaimTypes.Role;

    [JsonPropertyName("claims")] public IEnumerable<ClientPrincipalClaim> Claims { get; set; } = default!;

    private const string AzureFunctionsNameClaimType = "name";
    private const string AzureFunctionsRoleClaimType = "roles";

    private Func<ClientPrincipalClaim, Claim> DefaultClaimTransformer => (cpClaim) =>
    {
        var type = cpClaim.Type;
        switch (type)
        {
            case AzureFunctionsNameClaimType:
                type = RoleClaimType;
                break;
            case AzureFunctionsRoleClaimType:
                type = RoleClaimType;
                break;
        }

        return new Claim(type, cpClaim.Value);
    };

    public ClaimsPrincipal ToClaimsPrincipal(Func<ClientPrincipalClaim, Claim>? claimTransformer = null)
    {
        if (claimTransformer == null)
        {
            claimTransformer = DefaultClaimTransformer;
        }
        
        var identity = new ClaimsIdentity(IdentityProvider);
        identity.AddClaims(Claims.Select(c => claimTransformer(c)));
        
        return new ClaimsPrincipal(identity);
    }

    public static ClientPrincipal? CreateFromRequestData(HttpRequestData requestData)
    {
        if (!requestData.Headers.TryGetValues("x-ms-client-principal", out var header)) return default;
        
        var data = header.First();
        var decoded = Convert.FromBase64String(data);
        var json = Encoding.UTF8.GetString(decoded);
        return JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public static ClaimsPrincipal? ClaimsPrincipalFromRequestData(HttpRequestData requestData, Func<ClientPrincipalClaim, Claim>? claimTransformer = null)
    {
        var principal = CreateFromRequestData(requestData);
        if (principal == null)
        {
            return default;
        }
        return principal.ToClaimsPrincipal(claimTransformer);
    }
}