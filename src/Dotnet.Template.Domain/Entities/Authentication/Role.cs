using Dotnet.Template.Domain.Interfaces.Domain;
using Dotnet.Template.Domain.Interfaces.Domain.Auditing;

namespace Dotnet.Template.Domain.Entities.Authentication;

public class Role : IEntity, ICreationAudit
{
    public Guid Id { get; init; } = Guid.CreateVersion7();

    // Human-friendly identifier (e.g., "SuperAdmin", "TenantAdmin", "SubAdmin", "AppUser")
    public required string Name { get; init; }
    public string? Description { get; init; }

    // Navigations
    public ICollection<UserRole> UserRoles { get; init; } = [];
    public ICollection<RolePermission> RolePermissions { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public Guid CreatedBy { get; init; }
}
