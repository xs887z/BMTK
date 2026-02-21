namespace Obrasheniya.Application.Services;

public class LexicomAnalysisResult
{
    public string Category { get; set; } = "Общее";
    public string Priority { get; set; } = "normal";
    public List<string> Tags { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
}

public interface ILexicomService
{
    Task<LexicomAnalysisResult> AnalyzeAsync(string text);
}