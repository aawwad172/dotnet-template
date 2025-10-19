namespace Dotnet.Template.Domain.Interfaces.Domain.Auditing;


public interface IModificationAudit
{
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}

