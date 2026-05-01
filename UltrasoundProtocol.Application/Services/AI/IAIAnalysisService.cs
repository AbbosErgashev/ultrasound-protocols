namespace UltrasoundProtocol.Application.Services.AI;

public interface IAIAnalysisService
{
    Task<AIAnalysisResult> AnalyzeImageAsync(byte[] imageData, string bodyPart, string? doctorFindings = null);
    Task<AIAnalysisResult> AnalyzeTextAsync(string findings, string bodyPart);
}

public class AIAnalysisResult
{
    public bool Success { get; set; }
    public string Analysis { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}
