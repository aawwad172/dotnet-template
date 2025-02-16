using Dotnet.Template.Domain.Exceptions;

using Microsoft.Extensions.Configuration;

namespace Dotnet.Template.Application.Utilities;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Retrieves a required configuration value.
    /// It first checks the configuration (e.g., appsettings.json) and then falls back to environment variables.
    /// Throws an InvalidOperationException if the key is not found in either.
    /// </summary>
    /// <param name="configuration">The IConfiguration instance.</param>
    /// <param name="key">The key of the configuration value.</param>
    /// <returns>The configuration value as a string.</returns>
    public static string GetRequiredSetting(this IConfiguration configuration, string key)
    {
        // Try to get the value from the configuration (e.g., appsettings.json)
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            // Fallback to environment variables if not found in configuration
            value = Environment.GetEnvironmentVariable(key);
        }

        // If still not found, throw an exception
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new EnvironmentVariableNotSetException($"Missing required configuration key: {key}");
        }

        return value;
    }
}
