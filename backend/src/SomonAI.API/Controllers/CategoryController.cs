namespace SomonAI.API.Controllers;

[ApiController]
[Route("api/categories")]
[ApiConventionType(typeof(CustomApiConventions))]
public class CategoryController(
    ICategoryService categoryService,
    ILanguageProvider languageProvider) : ControllerBase
{

    /// <summary>
    /// Get all active categories
    /// </summary>
    /// <returns>List of categories with localized names</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all categories",
        Description = "Retrieves all active categories with localized names based on Accept-Language header",
        Tags = ["Categories"]
    )]
    [SwaggerResponse(200, "Categories retrieved successfully", typeof(ApiResponse<List<CategoryDto>>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var language = languageProvider.GetCurrentLanguage();
        var result = await categoryService.GetAllAsync(language);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    /// <param name="id">MongoDB ObjectId of the category</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Category details</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get category by ID",
        Description = "Retrieves a single category by its MongoDB ObjectId",
        Tags = ["Categories"]
    )]
    [SwaggerResponse(200, "Category found",
        typeof(ApiResponse<CategoryDto>))]
    [SwaggerResponse(400, "Invalid ID format")]
    [SwaggerResponse(404, "Category not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> GetById(
        [FromRoute] string id,
        CancellationToken cancellationToken = default)
    {
        var language = languageProvider.GetCurrentLanguage();
        var result = await categoryService.GetByIdAsync(id, language);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get category by slug
    /// </summary>
    /// <param name="slug">URL-friendly category slug (e.g., "cars", "real-estate")</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Category details</returns>
    [HttpGet("slug/{slug}")]
    [SwaggerOperation(
        Summary = "Get category by slug",
        Description = "Retrieves a category by its URL-friendly slug",
        Tags = ["Categories"]
    )]
    [SwaggerResponse(200, "Category found",
        typeof(ApiResponse<CategoryDto>))]
    [SwaggerResponse(400, "Invalid slug")]
    [SwaggerResponse(404, "Category not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> GetBySlug(
        [FromRoute] string slug, CancellationToken cancellationToken = default)
    {
        var language = languageProvider.GetCurrentLanguage();
        var result = await categoryService.GetBySlugAsync(slug, language);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get detailed category with all language variants
    /// </summary>
    /// <param name="id">MongoDB ObjectId of the category</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Category with all translations (for admin use)</returns>
    [HttpGet("{id}/details")]
    [SwaggerOperation(
        Summary = "Get category details (all languages)",
        Description = "Retrieves category with all language variants (Russian, Tajik, English) - for admin purposes",
        Tags = ["Categories"]
    )]
    [SwaggerResponse(200, "Category details retrieved",
        typeof(ApiResponse<CategoryDetailDto>))]
    [SwaggerResponse(400, "Invalid ID format")]
    [SwaggerResponse(404, "Category not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> GetDetails(
        [FromRoute] string id,
        CancellationToken cancellationToken = default)
    {
        var result = await categoryService.GetDetailAsync(id);
        return result.ToActionResult();
    }
}