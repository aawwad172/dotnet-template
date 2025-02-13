using Dotnet.Template.Domain.Exceptions;

namespace Dotnet.Template.Infrastructure.Persistence;

public class EnvironmentVariableExtensions
{
    public static string GetRequiredEnvVariable(string variableName)
    {
        string? value = Environment.GetEnvironmentVariable(variableName);
        if (string.IsNullOrEmpty(value))
            throw new EnvironmentVariableNotSetException(variableName);

        return value;
    }
}
