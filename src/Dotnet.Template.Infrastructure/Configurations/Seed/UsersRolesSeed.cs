using Dotnet.Template.Domain.Entities.Authentication;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dotnet.Template.Infrastructure.Configurations.Seed;

public class UsersRolesSeed : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasData([
            // 1. Link the Initial Admin User to the SuperAdmin Role
            new UserRole
            {
                // The Composite Primary Key is UserId and RoleId
                UserId = AuthSeedConstants.InitialAdminUserId,
                RoleId = AuthSeedConstants.RoleIdSuperAdmin,
            }

            // Add other initial user/role links here if necessary (e.g., Guest User, Test User)
            /*
            new UserRole 
            {
                UserId = AuthSeedConstants.GuestUserId,
                RoleId = AuthSeedConstants.RoleIdUser
            }
            */
        ]);
    }
}
