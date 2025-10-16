using Dotnet.Template.Application.Utilities;
using Dotnet.Template.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace Dotnet.Template.Infrastructure.Configurations.Seed;

public class UsersSeed(IConfiguration configuration) : IEntityTypeConfiguration<User>
{
    private readonly IConfiguration _configuration = configuration;
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasData(
            [
                new User
                {
                    Id = AuthSeedConstants.SystemUserId,
                    FirstName = "system",
                    LastName = "system",
                    Username = "system",
                    Email = "system@example.com",
                    PasswordHash = _configuration.GetRequiredSetting("Security:SystemAdminPasswordHash"), // Hash doesn't matter, but needs a value
                    SecurityStamp = AuthSeedConstants.SystemSecurityStampGuid,
                    IsActive = true,
                    IsVerified = true,
                    IsDeleted = false,
                    // Auditing: System created by System
                    CreatedAt = AuthSeedConstants.SeedDateUtc,
                    CreatedBy = AuthSeedConstants.SystemUserId,
                },
                new User {
                    Id = AuthSeedConstants.InitialAdminUserId,
                    FirstName = "Initial",
                    LastName = "Admin",
                    Username = "admin",
                    Email = "aawwad172@gmail.com",
                    PasswordHash = _configuration.GetRequiredSetting("Security:InitialAdminPasswordHash"),
                    SecurityStamp = AuthSeedConstants.AdminSecurityStampGuid,
                    IsActive = true,
                    IsVerified = true,
                    IsDeleted = false,
                    CreatedAt = AuthSeedConstants.SeedDateUtc,
                    CreatedBy = AuthSeedConstants.SystemUserId,
                }
            ]
        );
    }
}
