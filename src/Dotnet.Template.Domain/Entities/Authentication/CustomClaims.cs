using System.Security.Claims;

namespace Dotnet.Template.Domain.Entities.Authentication;

/// <summary>
/// Defines custom claim types used in the application's JWT for authorization.
/// </summary>
public static class CustomClaims
{
    // This is the key we use to store all granular permissions in the JWT payload.
    public const string Permission = "Permission";

    // This is the default .NET claim type for roles, often used with [Authorize(Roles="...")]
    // ClaimTypes.Role is defined in System.Security.Claims but it's good practice to list related claims
    public const string Role = ClaimTypes.Role;

    // Other custom claims can go here, like user's full name, etc.
    public const string FullName = "full_name";
}
