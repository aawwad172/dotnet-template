namespace Dotnet.Template.Domain.Interfaces.Auditing;


public interface IModificationAudited
{
    public DateTime UpdatedAt { get; set; }
    public Ulid UpdatedBy { get; set; }
}

