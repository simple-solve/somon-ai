namespace SomonAI.Lib.Configuration;

public class GeminiSettings
{
    public const string SectionName= "Gemini";
    /// <summary>Google Generative Language API key (store in secrets/env).</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Model name, напр. "gemini-2.0-flash".</summary>
    public string Model { get; set; } = "gemini-2.0-flash";

    /// <summary>API endpoint base, напр. "https://generativelanguage.googleapis.com".</summary>
    public string Endpoint { get; set; } = "https://generativelanguage.googleapis.com";
}