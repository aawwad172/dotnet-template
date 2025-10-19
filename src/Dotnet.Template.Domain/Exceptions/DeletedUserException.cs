namespace Dotnet.Template.Domain.Exceptions;

public class DeletedUserException : Exception
{
    public DeletedUserException(string? message) : base(message)
    {
    }

    public DeletedUserException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
