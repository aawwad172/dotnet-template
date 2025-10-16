using Dotnet.Template.Domain.Interfaces.Domain;
using Dotnet.Template.Domain.Interfaces.Domain.Auditing;

namespace Dotnet.Template.Domain.Entities.Authentication;

public class Permission : IBaseEntity
{
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <summary>
    /// Stable unique key used in code/policies, e.g. "Users.Read", "Roles.Write".
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Optional human description for admin UI.
    /// </summary>
    public string? Description { get; set; }

    // Navigations
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public DateTime CreatedAt { get; init; }
    public Guid CreatedBy { get; init; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}

