using Dotnet.Template.Domain.Entities.Authentication;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dotnet.Template.Infrastructure.Configurations.Seed;

public class RolesPermissionsSeed : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasData([
            // -----------------------------------------------------------------------------------
            // 1. SuperAdmin (The system doesn't strictly need these, but it's good practice)
            //    We link SA to all initial permissions.
            // -----------------------------------------------------------------------------------
            new RolePermission
            {
                RoleId = AuthSeedConstants.RoleIdSuperAdmin,
                PermissionId = AuthSeedConstants.PermissionIdUserRead
            },
            new RolePermission
            {
                RoleId = AuthSeedConstants.RoleIdSuperAdmin,
                PermissionId = AuthSeedConstants.PermissionIdPostApprove
            },

            // -----------------------------------------------------------------------------------
            // 2. Admin Role Permissions
            // -----------------------------------------------------------------------------------
            // Admin can approve posts
            new RolePermission
            {
                RoleId = AuthSeedConstants.RoleIdAdmin,
                PermissionId = AuthSeedConstants.PermissionIdPostApprove
            },
            // Admin can read all user accounts
            new RolePermission
            {
                RoleId = AuthSeedConstants.RoleIdAdmin,
                PermissionId = AuthSeedConstants.PermissionIdUserRead
            },

            // -----------------------------------------------------------------------------------
            // 3. Standard User Role Permissions
            // -----------------------------------------------------------------------------------
            // User can read their own profile (User.Read is often filtered by the service layer later)
            new RolePermission
            {
                RoleId = AuthSeedConstants.RoleIdUser,
                PermissionId = AuthSeedConstants.PermissionIdUserRead
            }
        ]);
    }
}
