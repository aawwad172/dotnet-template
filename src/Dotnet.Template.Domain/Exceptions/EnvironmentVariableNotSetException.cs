namespace Dotnet.Template.Domain.Exceptions;

public class EnvironmentVariableNotSetException(string variableName) : Exception($"Environment variable '{variableName}' is not set.")
{
    /// <summary>
    /// Gets the name of the environment variable that is not set.
    /// </summary>
    public string VariableName { get; } = variableName;
}
