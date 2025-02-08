using ProgressPath.Domain.Exceptions;

namespace ProgressPath.Infrastructure.Persistence;

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
    public static string GetDBConnectionString() => $"Host={GetRequiredEnvVariable("DB_HOST")};Port={GetRequiredEnvVariable("DB_PORT")};Database={GetRequiredEnvVariable("DB_NAME")};Username={GetRequiredEnvVariable("DB_USER")};Password={GetRequiredEnvVariable("DB_PASSWORD")}";

    private static string GetRequiredEnvVariable(string variableName)
    {
        string? value = Environment.GetEnvironmentVariable(variableName);
        if (string.IsNullOrEmpty(value)) throw new EnvironmentVariableNotSetException(variableName);
        return value;
    }
}
