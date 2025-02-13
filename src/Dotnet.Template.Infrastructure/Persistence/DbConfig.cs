using Dotnet.Template.Domain.Exceptions;

using Microsoft.Extensions.Configuration;

namespace Dotnet.Template.Infrastructure.Persistence;

/// <summary>
/// Represents the configuration settings for connecting to the database and Redis.
/// </summary>
public class DbConfig
{
    /// <summary>
    /// Retrieves the connection string for the database using the provided configuration.
    /// </summary>
    /// <returns>
    /// A connection string formatted for use with a PostgreSQL database.
    /// </returns>
    /// <exception cref="EnvironmentVariableNotSetException">
    /// Thrown when an expected environment variable is not set.
    /// </exception>
    public static string GetDBConnectionString() => $"Host={EnvironmentVariableExtensions.GetRequiredEnvVariable("DB_HOST")};Port={EnvironmentVariableExtensions.GetRequiredEnvVariable("DB_PORT")};Database={EnvironmentVariableExtensions.GetRequiredEnvVariable("DB_NAME")};Username={EnvironmentVariableExtensions.GetRequiredEnvVariable("DB_USER")};Password={EnvironmentVariableExtensions.GetRequiredEnvVariable("DB_PASSWORD")}";
}
