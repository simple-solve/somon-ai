using System.Net.Http.Headers;
using SomonAI.Lib.Templates;
using BuildingBlocks.Extensions.Http;

namespace SomonAI.Lib.Services;

public class GeminiService(
    IHttpClientFactory httpClientFactory,
    IOptions<GeminiSettings> settings,
    IOptions<FileStorageSettings> fileSettings,
    ILanguageProvider languageProvider) : IGeminiService
{
    private readonly GeminiSettings _settings = settings.Value;
    private readonly FileStorageSettings _fileSettings = fileSettings.Value;

    public async Task<Result<GeminiGenerateResponse?>> GenerateAsync(
        GeminiGenerateRequest request,
        CancellationToken cancellationToken = default)
    {
        var files = request.Files ?? Array.Empty<IFormFile>();
        var validation = ValidateFiles(files);
        if (!validation.IsSuccess)
            return Result<GeminiGenerateResponse?>.Failure(validation.Error);

        var mediaList = files.Length > 0
            ? string.Join(", ", files.Select(f => f.FileName))
            : "no media provided";

        var prompt = PromptBuilder.BuildPrompt(
            languageProvider.GetCurrentLanguage().GetDisplayName(),
            request.ClientPrompt ?? string.Empty,
            mediaList);

        var parts = new List<object> { new { text = prompt } };

        foreach (var file in files)
        {
            string base64;
            await using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms, cancellationToken);
                base64 = Convert.ToBase64String(ms.ToArray());
            }

            parts.Add(new
            {
                inline_data = new
                {
                    mime_type = GetMimeType(file), // fallback по расширению
                    data = base64
                }
            });
        }

        var payload = new
        {
            contents = new[]
            {
                new { parts }
            }
        };

        string url =
            $"{_settings.Endpoint.TrimEnd('/')}/v1beta/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";

        using var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var result = await HttpClientHelper.PostAsync<object, dynamic>(
            client,
            url,
            payload,
            cancellationToken);

        if (!result.IsSuccess)
            return Result<GeminiGenerateResponse?>.Failure(result.Error);

        var raw = JsonConvert.SerializeObject(result.Value);
        return Result<GeminiGenerateResponse?>.Success(new GeminiGenerateResponse { RawResponse = raw });
    }

    private string GetMimeType(IFormFile file)
    {
        var ct = file.ContentType?.Trim();
        if (!string.IsNullOrWhiteSpace(ct) && !ct.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
            return ct;

        var ext = Path.GetExtension(file.FileName).Trim().ToLowerInvariant();
        return ext switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".webp" => "image/webp",
            ".heic" => "image/heic",
            ".heif" => "image/heif",
            ".gif" => "image/gif",
            ".mp4" => "video/mp4",
            ".mov" => "video/quicktime",
            ".avi" => "video/x-msvideo",
            ".webm" => "video/webm",
            ".mkv" => "video/x-matroska",
            _ => "application/octet-stream"
        };
    }

    private Result<bool> ValidateFiles(IEnumerable<IFormFile> files)
    {
        var allowedImages = _fileSettings.AllowedImageExtensions
            .Select(e => e.Trim().ToLowerInvariant())
            .ToHashSet();
        var allowedVideos = _fileSettings.AllowedVideoExtensions
            .Select(e => e.Trim().ToLowerInvariant())
            .ToHashSet();

        foreach (var file in files)
        {
            var ext = Path.GetExtension(file.FileName).Trim().ToLowerInvariant();
            bool isImage = allowedImages.Contains(ext);
            bool isVideo = allowedVideos.Contains(ext);

            if (!isImage && !isVideo)
                return Result<bool>.Failure(ResultError.UnsupportedMediaType($"Extension not allowed: {ext}"));

            var sizeMb = file.Length / 1024f / 1024f;
            if (isImage && sizeMb > _fileSettings.MaxImageSizeMb)
                return Result<bool>.Failure(ResultError.BadRequest($"Image too large: {file.FileName} ({sizeMb:F1} MB)"));

            if (isVideo && sizeMb > _fileSettings.MaxVideoSizeMb)
                return Result<bool>.Failure(ResultError.BadRequest($"Video too large: {file.FileName} ({sizeMb:F1} MB)"));
        }

        return Result<bool>.Success(true);
    }
}