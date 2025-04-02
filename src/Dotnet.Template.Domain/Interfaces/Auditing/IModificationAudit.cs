namespace Dotnet.Template.Domain.Interfaces.Auditing;


public interface IModificationAudit
{
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}

