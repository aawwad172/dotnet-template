using Dotnet.Template.Domain.Entities.Authentication;
using Dotnet.Template.Domain.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dotnet.Template.Infrastructure.Configurations.Seed;

public class PermissionsSeed : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        // Todo: Add the desired Permissions.
        builder.HasData([
            new Permission
            {
                Id = AuthSeedConstants.PermissionIdUserRead,
                Name = PermissionsEnum.UserRead,
                Description = "some description",
                CreatedAt = AuthSeedConstants.SeedDateUtc,
                CreatedBy = AuthSeedConstants.SystemUserId
            },
            new Permission
            {
                Id = AuthSeedConstants.PermissionIdPostApprove,
                Name = PermissionsEnum.PostApprove,
                CreatedAt = AuthSeedConstants.SeedDateUtc,
                CreatedBy = AuthSeedConstants.SystemUserId
            }
        ]);
    }
}
