namespace SomonAI.Lib.DTOs;

public class GeminiGenerateRequestDto
{ 
    public string? ClientPrompt { get; set; }
    public IFormFile[]? Files { get; set; }
}