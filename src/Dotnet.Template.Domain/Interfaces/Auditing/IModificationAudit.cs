namespace Dotnet.Template.Domain.Interfaces.Auditing;


public interface IModificationAudit
{
    public DateTime UpdatedAt { get; set; }
    public Ulid UpdatedBy { get; set; }
}

