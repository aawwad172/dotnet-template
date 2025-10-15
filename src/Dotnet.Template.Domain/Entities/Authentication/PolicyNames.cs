namespace Dotnet.Template.Domain.Entities.Authentication;

public static class PolicyNames
{
    public const string UserOnly = "UserPolicy";
    public const string AdminOrAbove = "AdminOrAbovePolicy";
    public const string SuperAdminOnly = "SuperAdminPolicy";

    // Permission Policy Prefix (used by your custom [HasPermission] attribute)
    public const string PermissionPrefix = "Permission:";
}
