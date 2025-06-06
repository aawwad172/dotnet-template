using System.ComponentModel.DataAnnotations;

using Dotnet.Template.Domain.Enums;
using Dotnet.Template.Domain.Interfaces;
using Dotnet.Template.Domain.Interfaces.Auditing;
namespace Dotnet.Template.Domain.Entities;


public record User : IEntity, ICreationAudit, IModificationAudit
{
    [Key] public required Guid Id { get; set; }
    [MaxLength(50)] public required string FirstName { get; set; }
    [MaxLength(50)] public required string LastName { get; set; }
    [EmailAddress] public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Username { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required Guid UpdatedBy { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public RolesEnum Role { get; set; }
}
