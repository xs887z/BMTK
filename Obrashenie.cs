namespace Obrasheniya.Domain.Entities;

public class Obrashenie
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public ObrashenieStatus Status { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? LexicomAnalysis { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public virtual ICollection<ObrashenieResponse> Responses { get; set; } = new List<ObrashenieResponse>();
}

public enum ObrashenieStatus
{
    New = 0,
    InProgress = 1,
    Processed = 2,
    Rejected = 3
}