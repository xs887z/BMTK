using System.Text.Json;

namespace Obrasheniya.Application.Services;

public class LexicomService : ILexicomService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    
    public LexicomService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    public async Task<LexicomAnalysisResult> AnalyzeAsync(string text)
    {
        try
        {
            var apiUrl = _configuration["Lexicom:ApiUrl"];
            var apiKey = _configuration["Lexicom:ApiKey"];
            
            var request = new
            {
                text = text,
                options = new { categories = true, tags = true, sentiment = true }
            };
            
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var response = await _httpClient.PostAsJsonAsync(apiUrl, request);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return ParseLexicomResponse(content);
            }
            
            return FallbackAnalysis(text);
        }
        catch
        {
            return FallbackAnalysis(text);
        }
    }
    
    private static LexicomAnalysisResult ParseLexicomResponse(string jsonResponse)
    {
        var result = new LexicomAnalysisResult();
        
        try
        {
            using var document = JsonDocument.Parse(jsonResponse);
            var root = document.RootElement;
            
            result.Category = root.GetProperty("category").GetString() ?? "Общее";
            result.Priority = root.GetProperty("priority").GetString() ?? "normal";
            result.Summary = root.GetProperty("summary").GetString() ?? string.Empty;
            
            foreach (var tag in root.GetProperty("tags").EnumerateArray())
            {
                result.Tags.Add(tag.GetString() ?? string.Empty);
            }
        }
        catch
        {
            return FallbackAnalysis("");
        }
        
        return result;
    }
    
    private static LexicomAnalysisResult FallbackAnalysis(string text)
    {
        var result = new LexicomAnalysisResult();
        
        if (text.Contains("срочно") || text.Contains("авария") || text.Contains("опасно"))
            result.Priority = "high";
        
        if (text.Contains("ЖКХ") || text.Contains("вода") || text.Contains("отопление"))
            result.Category = "ЖКХ";
        else if (text.Contains("документ") || text.Contains("справка") || text.Contains("паспорт"))
            result.Category = "Документы";
        else if (text.Contains("дорог") || text.Contains("транспорт") || text.Contains("светофор"))
            result.Category = "Транспорт";
        
        return result;
    }
}