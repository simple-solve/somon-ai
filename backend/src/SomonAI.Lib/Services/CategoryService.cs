namespace SomonAI.Lib.Services;

/// <summary>
/// Category service implementation with Result Pattern and structured logging
/// </summary>
public class CategoryService(
    IMongoDbContext context,
    ILogger<CategoryService> logger) : ICategoryService
{
    /// <summary>
    /// Get all active categories
    /// </summary>
    public async Task<Result<List<CategoryDto>>> GetAllAsync(Language language)
    {
        var sw = Stopwatch.StartNew();
        const string operation = nameof(GetAllAsync);

        try
        {
            logger.OperationStarted(operation, DateTimeOffset.UtcNow);
            logger.OperationInfo(operation, $"Retrieving all active categories for language: {language.GetCode()}");

            var categories = await context.Categories
                .Find(c => c.IsActive)
                .SortBy(c => c.DisplayOrder)
                .ToListAsync();

            if (categories.Count == 0)
            {
                logger.OperationWarning(operation, "No active categories found");
            }

            var dtos = categories.Select(c => MapToDto(c, language)).ToList();

            logger.OperationInfo(operation, $"Retrieved {dtos.Count} categories");
            logger.OperationCompleted(operation, DateTimeOffset.UtcNow, sw.ElapsedMilliseconds);

            return Result<List<CategoryDto>>.Success(dtos);
        }
        catch (MongoException ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"MongoDB error: {ex.Message}");
            return Result<List<CategoryDto>>.Failure(
                ResultError.InternalServerError("Database error while retrieving categories"));
        }
        catch (Exception ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"Unexpected error: {ex.Message}");
            return Result<List<CategoryDto>>.Failure(
                ResultError.InternalServerError("Failed to retrieve categories"));
        }
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    public async Task<Result<CategoryDto>> GetByIdAsync(string id, Language language)
    {
        var sw = Stopwatch.StartNew();
        const string operation = nameof(GetByIdAsync);

        try
        {
            logger.OperationStarted(operation, DateTimeOffset.UtcNow);
            logger.OperationInfo(operation, $"Retrieving category by ID: {id}, Language: {language.GetCode()}");

            // Validate ObjectId format
            if (!ObjectId.TryParse(id, out _))
            {
                logger.OperationWarning(operation, $"Invalid ObjectId format: {id}");
                return Result<CategoryDto>.Failure(
                    ResultError.BadRequest($"Invalid category ID format: {id}"));
            }

            var category = await context.Categories
                .Find(c => c.Id == id && c.IsActive)
                .FirstOrDefaultAsync();

            if (category == null)
            {
                logger.OperationWarning(operation, $"Category not found: {id}");
                return Result<CategoryDto>.Failure(
                    ResultError.NotFound($"Category with ID '{id}' not found"));
            }

            var dto = MapToDto(category, language);

            logger.OperationInfo(operation, $"Category found: {category.Slug}");
            logger.OperationCompleted(operation, DateTimeOffset.UtcNow, sw.ElapsedMilliseconds);

            return Result<CategoryDto>.Success(dto);
        }
        catch (MongoException ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"MongoDB error:  {ex.Message}");
            return Result<CategoryDto>.Failure(
                ResultError.InternalServerError("Database error while retrieving category"));
        }
        catch (Exception ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"Unexpected error: {ex.Message}");
            return Result<CategoryDto>.Failure(
                ResultError.InternalServerError("Failed to retrieve category"));
        }
    }

    /// <summary>
    /// Get category by slug
    /// </summary>
    public async Task<Result<CategoryDto>> GetBySlugAsync(string slug, Language language)
    {
        var sw = Stopwatch.StartNew();
        const string operation = nameof(GetBySlugAsync);

        try
        {
            logger.OperationStarted(operation, DateTimeOffset.UtcNow);
            logger.OperationInfo(operation, $"Retrieving category by slug: {slug}, Language: {language.GetCode()}");

            // Validate slug
            if (string.IsNullOrWhiteSpace(slug))
            {
                logger.OperationWarning(operation, "Slug is null or empty");
                return Result<CategoryDto>.Failure(
                    ResultError.BadRequest("Category slug cannot be empty"));
            }

            var category = await context.Categories
                .Find(c => c.Slug == slug.ToLower() && c.IsActive)
                .FirstOrDefaultAsync();

            if (category == null)
            {
                logger.OperationWarning(operation, $"Category not found: {slug}");
                return Result<CategoryDto>.Failure(
                    ResultError.NotFound($"Category '{slug}' not found"));
            }

            var dto = MapToDto(category, language);

            logger.OperationInfo(operation, $"Category found: {category.Id}");
            logger.OperationCompleted(operation, DateTimeOffset.UtcNow, sw.ElapsedMilliseconds);

            return Result<CategoryDto>.Success(dto);
        }
        catch (MongoException ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"MongoDB error: {ex.Message}");
            return Result<CategoryDto>.Failure(
                ResultError.InternalServerError("Database error while retrieving category"));
        }
        catch (Exception ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"Unexpected error:  {ex.Message}");
            return Result<CategoryDto>.Failure(
                ResultError.InternalServerError("Failed to retrieve category"));
        }
    }

    /// <summary>
    /// Get detailed category (with all 3 languages)
    /// </summary>
    public async Task<Result<CategoryDetailDto>> GetDetailAsync(string id)
    {
        var sw = Stopwatch.StartNew();
        const string operation = nameof(GetDetailAsync);

        try
        {
            logger.OperationStarted(operation, DateTimeOffset.UtcNow);
            logger.OperationInfo(operation, $"Retrieving detailed category:  {id}");

            // Validate ObjectId format
            if (!ObjectId.TryParse(id, out _))
            {
                logger.OperationWarning(operation, $"Invalid ObjectId format: {id}");
                return Result<CategoryDetailDto>.Failure(
                    ResultError.BadRequest($"Invalid category ID format: {id}"));
            }

            var category = await context.Categories
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (category == null)
            {
                logger.OperationWarning(operation, $"Category not found: {id}");
                return Result<CategoryDetailDto>.Failure(
                    ResultError.NotFound($"Category with ID '{id}' not found"));
            }

            var dto = new CategoryDetailDto
            {
                Id = category.Id,
                Slug = category.Slug,
                NameRu = category.NameRu,
                NameTj = category.NameTj,
                NameEn = category.NameEn,
                DescriptionRu = category.DescriptionRu,
                DescriptionTj = category.DescriptionTj,
                DescriptionEn = category.DescriptionEn,
                Icon = category.Icon,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };

            logger.OperationInfo(operation, $"Category details retrieved: {category.Slug}");
            logger.OperationCompleted(operation, DateTimeOffset.UtcNow, sw.ElapsedMilliseconds);

            return Result<CategoryDetailDto>.Success(dto);
        }
        catch (MongoException ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"MongoDB error: {ex.Message}");
            return Result<CategoryDetailDto>.Failure(
                ResultError.InternalServerError("Database error while retrieving category details"));
        }
        catch (Exception ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"Unexpected error: {ex.Message}");
            return Result<CategoryDetailDto>.Failure(
                ResultError.InternalServerError("Failed to retrieve category details"));
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Map Category entity to CategoryDto with localization (supports 3 languages)
    /// </summary>
    private static CategoryDto MapToDto(Category category, Language language)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Slug = category.Slug,
            Name = language switch
            {
                Language.Russian => category.NameRu,
                Language.Tajik => category.NameTj,
                Language.English => category.NameEn,
                _ => category.NameRu
            },
            Description = language switch
            {
                Language.Russian => category.DescriptionRu,
                Language.Tajik => category.DescriptionTj,
                Language.English => category.DescriptionEn,
                _ => category.DescriptionRu
            },
            Icon = category.Icon,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    #endregion
}