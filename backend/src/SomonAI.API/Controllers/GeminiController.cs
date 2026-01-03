namespace SomonAI.API.Controllers;

[ApiController]
[Route("api/ai")]
[ApiConventionType(typeof(CustomApiConventions))]
public class GeminiController(IGeminiService geminiService) : ControllerBase
{
    /// <summary>
    /// Send prompt and optional media to Gemini and get raw JSON response.
    /// </summary>
    [HttpPost("gemini")]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(
        Summary = "Generate via Gemini",
        Description = "Sends prompt and optional files to Gemini. Returns raw JSON response.",
        Tags = ["AI"]
    )]
    [SwaggerResponse(200, "Response received", typeof(ApiResponse<GeminiGenerateResponse>))]
    [SwaggerResponse(400, "Invalid input")]
    [SwaggerResponse(415, "Unsupported media type")]
    public async Task<IActionResult> Generate(
        [FromForm] GeminiGenerateRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        var request = new GeminiGenerateRequest
        {
            ClientPrompt = dto.ClientPrompt,
            Files = dto.Files
        };

        var result = await geminiService.GenerateAsync(request, cancellationToken);
        return result.ToActionResult();
    }
}