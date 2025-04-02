namespace Dotnet.Template.Domain.Exceptions;

public class CustomValidationException : Exception
{
    public IEnumerable<string> Errors { get; }

    public CustomValidationException() : base("Validation failed.")
    {
        Errors = Array.Empty<string>();
    }

    public CustomValidationException(string message) : base(message)
    {
        Errors = Array.Empty<string>();
    }

    public CustomValidationException(string message, IEnumerable<string> errors) : base(message)
    {
        Errors = errors;
    }

    public CustomValidationException(string message, Exception innerException) : base(message, innerException)
    {
        Errors = Array.Empty<string>();
    }
}
