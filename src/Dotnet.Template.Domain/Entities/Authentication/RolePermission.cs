namespace Dotnet.Template.Domain.Entities.Authentication;

public class RolePermission
{
    // Composite key will be configured in EF (RoleId + PermissionId)
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}
