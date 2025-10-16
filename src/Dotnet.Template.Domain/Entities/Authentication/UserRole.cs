using Dotnet.Template.Domain.Interfaces.Domain.Auditing;

namespace Dotnet.Template.Domain.Entities.Authentication;

public class UserRole
{
    public required Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Which role
    public required Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;          // Role entity will be added next
}
