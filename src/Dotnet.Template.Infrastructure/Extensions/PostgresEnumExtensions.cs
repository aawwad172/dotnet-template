using Dotnet.Template.Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace Dotnet.Template.Infrastructure.Extensions;

public static class PostgresEnumExtensions
{
    public static ModelBuilder RegisterPostgresEnums(this ModelBuilder builder)
    {
        // Register your enums here
        builder.HasPostgresEnum<RolesEnum>();
        // Add other enum registrations if needed

        return builder;
    }
}
