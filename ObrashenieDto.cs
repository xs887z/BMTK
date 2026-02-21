namespace Obrasheniya.Application.DTOs;

public class CreateObrashenieDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class ObrashenieDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public ObrashenieStatus Status { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? LexicomAnalysis { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserDto User { get; set; } = null!;
}