using Dotnet.Template.Domain.Interfaces.Domain;
using Dotnet.Template.Domain.Interfaces.Domain.Auditing;

namespace Dotnet.Template.Domain.Entities.Authentication;

public class RefreshToken : IEntity, ICreationAudit
{
    // Identity
    public Guid Id { get; init; } = Guid.CreateVersion7();

    // Link to user/session family
    public required Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public required Guid TokenFamilyId { get; set; }  // all rotations in one login share this

    // Security (store only hashed token)
    public required string TokenHash { get; set; }     // Base64 or hex of the hashed token
    public required string PlaintextToken { get; set; } // <--- NOT MAPPED TO DB

    // Lifetime
    public required DateTime ExpiresAt { get; set; }   // UTC

    // Revocation / rotation
    public DateTime? RevokedAt { get; set; }           // UTC
    public string? ReasonRevoked { get; set; }         // "Rotated" | "Logout" | "ReuseDetected" | etc.
    public Guid? ReplacedByTokenId { get; set; }       // FK to next token in the rotation chain
    public RefreshToken? ReplacedByToken { get; set; }

    // Issuance metadata
    public string? SecurityStampAtIssue { get; set; }  // copy of user's SecurityStamp at issuance (optional but useful)

    // Auditing
    public required DateTime CreatedAt { get; init; }
    public required Guid CreatedBy { get; init; }       // usually = UserId

    // Convenience (not mapped)
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsExpired && !IsRevoked;
}
