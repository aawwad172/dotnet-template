using Dotnet.Template.Domain.Entities.Authentication;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dotnet.Template.Infrastructure.Configurations;

public class UserPermissionOverrideConfiguration : IEntityTypeConfiguration<UserPermissionOverride>
{
    public void Configure(EntityTypeBuilder<UserPermissionOverride> builder)
    {
        builder.ToTable("UserPermissionOverrides");

        builder.HasKey(upo => new { upo.UserId, upo.PermissionId }); // Define Composite PK

        // FKs
        builder.HasOne(x => x.User)
           .WithMany(u => u.PermissionOverrides)
           .HasForeignKey(x => x.UserId)
           .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Permission)
           .WithMany(x => x.UserOverrides) // no dedicated nav; harmless to omit
           .HasForeignKey(x => x.PermissionId)
           .OnDelete(DeleteBehavior.Cascade);

        // Perf indexes
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.PermissionId);

        // Auditing
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.CreatedBy).IsRequired();
    }
}
