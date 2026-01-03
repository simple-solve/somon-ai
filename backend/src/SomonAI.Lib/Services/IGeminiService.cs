namespace SomonAI.Lib.Services;

public interface IGeminiService
{
    Task<Result<GeminiGenerateResponse?>> GenerateAsync(
        GeminiGenerateRequest request,
        CancellationToken cancellationToken = default);
}