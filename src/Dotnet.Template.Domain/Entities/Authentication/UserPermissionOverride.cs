using Dotnet.Template.Domain.Interfaces.Domain.Auditing;

namespace Dotnet.Template.Domain.Entities.Authentication;

public class UserPermissionOverride : ICreationAudit
{
    // Who
    public required Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // What
    public required Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;


    // Decision
    public required bool IsGranted { get; set; }   // false = Deny (deny beats allow during evaluation)

    // Auditing
    public required DateTime CreatedAt { get; init; }
    public required Guid CreatedBy { get; init; }
}
