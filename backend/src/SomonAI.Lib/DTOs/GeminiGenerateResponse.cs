namespace SomonAI.Lib.DTOs;

/// <summary>Результат генерации: сырой JSON ответа Gemini.</summary>
public class GeminiGenerateResponse
{
    public string RawResponse { get; set; } = string.Empty;
}