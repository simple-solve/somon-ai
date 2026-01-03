namespace SomonAI.Lib.Services;

/// <summary>
/// Category service interface
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Get all active categories
    /// </summary>
    Task<Result<List<CategoryDto>>> GetAllAsync(Language language);

    /// <summary>
    /// Get category by ID
    /// </summary>
    Task<Result<CategoryDto>> GetByIdAsync(string id, Language language);

    /// <summary>
    /// Get category by slug
    /// </summary>
    Task<Result<CategoryDto>> GetBySlugAsync(string slug, Language language);

    /// <summary>
    /// Get detailed category (with both languages)
    /// </summary>
    Task<Result<CategoryDetailDto>> GetDetailAsync(string id);
}