namespace Dotnet.Template.Domain.Exceptions;

public class NotActiveUserExceptions : Exception
{
    public NotActiveUserExceptions(string? message) : base(message)
    {
    }

    public NotActiveUserExceptions(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
