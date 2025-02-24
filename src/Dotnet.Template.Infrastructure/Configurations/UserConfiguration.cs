using Dotnet.Template.Application.HelperServices;
using Dotnet.Template.Application.Interfaces.Services;
using Dotnet.Template.Application.Utilities;
using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Infrastructure.Convertors;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace Dotnet.Template.Infrastructure.Configurations;

public class UserConfiguration(EncryptionService encryptionService, IConfiguration configuration) : IEntityTypeConfiguration<User>
{
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly IConfiguration _configuration = configuration;
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .IsRequired()
            .HasConversion(UlidToStringConvertor.Instance);

        builder.Property(user => user.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(user => user.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.Email)
            .IsRequired();

        builder.Property(user => user.PasswordHash)
            .IsRequired();

        builder.Property(user => user.Username)
            .IsRequired();

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.Property(user => user.CreatedBy)
            .IsRequired()
            .HasConversion(UlidToStringConvertor.Instance);

        builder.Property(user => user.UpdatedAt)
            .IsRequired();

        builder.Property(user => user.UpdatedBy)
            .IsRequired()
            .HasConversion(UlidToStringConvertor.Instance);

        builder.HasMany(user => user.RefreshTokens)
            .WithOne(refreshToken => refreshToken.User)
            .HasForeignKey(refreshToken => refreshToken.Id);

        // Data Seeding
        builder.HasData(
            // Todo: Update this after adding the permisions to the User
            new User
            {
                Id = Ulid.Parse("00000000-0000-0000-0000-000000000000"),
                FirstName = "System",
                LastName = "Administrator",
                Email = "systemadmin@hms.com",
                PasswordHash = _encryptionService.HashPassword(_configuration.GetRequiredSetting("SystemAdminPassword")),
                Username = "systemadmin",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = Ulid.Parse("00000000-0000-0000-0000-000000000000"),
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = Ulid.Parse("00000000-0000-0000-0000-000000000000")
            }
        );
    }
}
