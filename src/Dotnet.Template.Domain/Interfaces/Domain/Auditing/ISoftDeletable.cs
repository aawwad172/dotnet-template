namespace Dotnet.Template.Domain.Interfaces.Domain.Auditing;

public interface ISoftDelete
{
    public bool IsDeleted { get; set; }
}
