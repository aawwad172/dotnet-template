namespace Dotnet.Template.Domain.Interfaces.Auditing;

public interface ICreationAudit
{
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
