namespace Dotnet.Template.Domain.Interfaces.Auditing;

public interface ICreationAudited
{
    public DateTime CreatedAt { get; set; }
    public Ulid CreatedBy { get; set; }
}
