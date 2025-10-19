using Dotnet.Template.Domain.Entities.Authentication;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dotnet.Template.Infrastructure.Configurations.Seed;

public class RolesSeed : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData([
             new Role
             {
                 Id = AuthSeedConstants.RoleIdSuperAdmin,
                 Name = "SuperAdmin",
                 Description = "Full unrestricted access.",
                 CreatedAt = AuthSeedConstants.SeedDateUtc,
                 CreatedBy = AuthSeedConstants.SystemUserId
             },
            new Role
            {
                Id = AuthSeedConstants.RoleIdAdmin,
                Name = "Admin",
                Description = "General administrative access.",
                CreatedAt = AuthSeedConstants.SeedDateUtc,
                CreatedBy = AuthSeedConstants.SystemUserId
            },
            new Role
            {
                Id = AuthSeedConstants.RoleIdUser,
                Name = "User",
                Description = "Standard registered user access.",
                CreatedAt = AuthSeedConstants.SeedDateUtc,
                CreatedBy = AuthSeedConstants.SystemUserId
            }
        ]);
    }
}
