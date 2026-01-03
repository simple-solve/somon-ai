namespace SomonAI.API.Controllers;

[ApiController]
[Route("api/products")]
[ApiConventionType(typeof(CustomApiConventions))]
public class ProductsController(
    IProductService productService,
    ILanguageProvider languageProvider) : ControllerBase
{
    /// <summary>
    /// Get all products with filtering and pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all products",
        Description = "Retrieves published products with optional category filter and pagination",
        Tags = ["Products"]
    )]
    [SwaggerResponse(200, "Products retrieved successfully", typeof(ApiResponse<List<ProductListDto>>))]
    [SwaggerResponse(400, "Invalid parameters")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? categoryId = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        CancellationToken cancellationToken = default)
    {
        var language = languageProvider.GetCurrentLanguage();
        var result = await productService.GetAllAsync(categoryId, language, skip, take);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get product by ID",
        Description = "Retrieves a single product by its MongoDB ObjectId.  Automatically increments view count.",
        Tags = ["Products"]
    )]
    [SwaggerResponse(200, "Product found", typeof(ApiResponse<ProductDto>))]
    [SwaggerResponse(400, "Invalid ID format")]
    [SwaggerResponse(404, "Product not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> GetById(
        [FromRoute] string id,
        CancellationToken cancellationToken = default)
    {
        var language = languageProvider.GetCurrentLanguage();
        var result = await productService.GetByIdAsync(id, language);
        return result.ToActionResult();
    }

    /// <summary>
    /// Create new product with files (images and videos)
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(
        Summary = "Create product with files",
        Description = "Creates a new product with images and/or videos.  Product is published immediately.",
        Tags = ["Products"]
    )]
    [SwaggerResponse(200, "Product created successfully", typeof(ApiResponse<ProductDto>))]
    [SwaggerResponse(400, "Invalid data")]
    [SwaggerResponse(404, "Category not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> Create(
        [FromForm] CreateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var result = await productService.CreateAsync(dto);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete product and all associated files
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete product",
        Description = "Permanently deletes a product and all its associated files (images and videos)",
        Tags = ["Products"]
    )]
    [SwaggerResponse(200, "Product deleted successfully", typeof(ApiResponse<bool>))]
    [SwaggerResponse(400, "Invalid ID format")]
    [SwaggerResponse(404, "Product not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> Delete(
        [FromRoute] string id,
        CancellationToken cancellationToken = default)
    {
        var result = await productService.DeleteAsync(id);
        return result.ToActionResult();
    }
}