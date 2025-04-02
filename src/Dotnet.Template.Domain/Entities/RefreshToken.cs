using Dotnet.Template.Domain.Interfaces;
using Dotnet.Template.Domain.Interfaces.Auditing;

namespace Dotnet.Template.Domain.Entities;

public class RefreshToken : IEntity, ICreationAudit
{
    public Guid Id { get; set; }
    public required string Token { get; set; }
    public required DateTime Expires { get; set; }
    private bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime? Revoked { get; set; }
    public Guid? ReplacedByToken { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;
    public required DateTime CreatedAt { get; set; }
    public required Guid CreatedBy { get; set; }

    // Navigation property back to User
    public User User { get; set; } = null!;
    public required Guid UserId { get; set; }
}
