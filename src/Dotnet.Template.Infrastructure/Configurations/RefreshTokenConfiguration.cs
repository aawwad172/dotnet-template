using Dotnet.Template.Domain.Entities.Authentication;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dotnet.Template.Infrastructure.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenHash).IsRequired().HasMaxLength(512);

        builder.Property(x => x.ReasonRevoked).HasMaxLength(128);
        builder.Property(x => x.SecurityStampAtIssue).HasMaxLength(128);

        builder.Ignore(rt => rt.PlaintextToken);

        builder.HasOne(x => x.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ReplacedByToken)
            .WithMany()
            .HasForeignKey(x => x.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.UserId, x.TokenFamilyId });
        builder.HasIndex(x => x.ExpiresAt);
        builder.HasIndex(x => x.TokenHash).IsUnique(); // fast lookup by presented token (after hashing)
    }
}

