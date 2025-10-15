namespace Dotnet.Template.Domain.Interfaces.Domain.Auditing;

public interface ICreationAudit
{
    public DateTime CreatedAt { get; init; }
    public Guid CreatedBy { get; init; }
}
