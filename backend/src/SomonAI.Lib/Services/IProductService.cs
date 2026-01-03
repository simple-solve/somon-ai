namespace SomonAI.Lib.Services;

/// <summary>
/// Product service interface
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Get all products with optional filtering
    /// </summary>
    Task<Result<List<ProductListDto>>> GetAllAsync(
        string? categoryId = null,
        Language? language = null,
        int skip = 0,
        int take = 20);

    /// <summary>
    /// Get product by ID
    /// </summary>
    Task<Result<ProductDto>> GetByIdAsync(string id, Language language);

    /// <summary>
    /// Create new product with files
    /// </summary>
    Task<Result<ProductDto>> CreateAsync(CreateProductDto dto);

    /// <summary>
    /// Delete product and all associated files
    /// </summary>
    Task<Result<bool>> DeleteAsync(string id);
}