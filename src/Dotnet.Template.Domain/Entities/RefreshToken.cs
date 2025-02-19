using Dotnet.Template.Domain.Interfaces;
using Dotnet.Template.Domain.Interfaces.Auditing;

namespace Dotnet.Template.Domain.Entities;

public class RefreshToken : IEntity, ICreationAudited
{
    public Ulid Id { get; set; }
    public required string Token { get; set; }
    public required DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime? Revoked { get; set; }
    public string? ReplacedByToken { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;
    public DateTime CreatedAt { get; set; }
    public Ulid CreatedBy { get; set; }

    // Navigation property back to User
    public User User { get; set; } = null!;
    public required Ulid UserId { get; set; }
}
