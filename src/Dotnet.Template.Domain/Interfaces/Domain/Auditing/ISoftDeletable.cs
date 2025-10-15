namespace Dotnet.Template.Domain.Interfaces.Domain.Auditing;

public interface ISoftDeletable
{
    public bool IsDeleted { get; set; }
}
