using Dotnet.Template.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dotnet.Template.Infrastructure.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(refreshToken => refreshToken.Id);

        builder.Property(refreshToken => refreshToken.Id)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.Token)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.Expires)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.ReplacedByToken)
            .IsRequired(false);

        builder.Property(refreshToken => refreshToken.CreatedAt)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.CreatedBy)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.UserId)
            .IsRequired();

        builder.HasOne(refreshToken => refreshToken.User)
            .WithMany(user => user.RefreshTokens)
            .HasForeignKey(refreshToken => refreshToken.UserId);
    }
}

