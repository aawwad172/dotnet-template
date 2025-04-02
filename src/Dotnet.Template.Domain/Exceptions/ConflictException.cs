namespace Dotnet.Template.Domain.Exceptions;

public class ConflictException : Exception
{
    public ConflictException()
               : base("A conflict occurred with the current state of the resource.")
    {
    }

    public ConflictException(string message)
        : base(message)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
