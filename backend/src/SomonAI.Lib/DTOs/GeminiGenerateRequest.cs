namespace SomonAI.Lib.DTOs;

public class GeminiGenerateRequest
{
    public string? ClientPrompt { get; set; }
    public IFormFile[]? Files { get; set; }
}