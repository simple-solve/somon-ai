namespace SomonAI.Lib.Services;

/// <summary>
/// Product service implementation with file upload support
/// </summary>
public class ProductService(
    IMongoDbContext context,
    ICategoryService categoryService,
    IFileService fileService,
    ILogger<ProductService> logger) : IProductService
{
    /// <summary>
    /// Get all products with optional filtering
    /// </summary>
    public async Task<Result<List<ProductListDto>>> GetAllAsync(
        string? categoryId = null,
        Language? language = null,
        int skip = 0,
        int take = 20)
    {
        var sw = Stopwatch.StartNew();
        const string operation = nameof(GetAllAsync);

        try
        {
            logger.OperationStarted(operation, DateTimeOffset.UtcNow);
            logger.OperationInfo(operation,
                $"Retrieving products:  CategoryId={categoryId}, Language={language?.GetCode()}, Skip={skip}, Take={take}");

            var filterBuilder = Builders<Product>.Filter;
            var filter = filterBuilder.Empty;

            // Filter by category
            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                if (!ObjectId.TryParse(categoryId, out _))
                {
                    logger.OperationWarning(operation, $"Invalid category ID format: {categoryId}");
                    return Result<List<ProductListDto>>.Failure(
                        ResultError.BadRequest($"Invalid category ID format: {categoryId}"));
                }

                filter &= filterBuilder.Eq(p => p.CategoryId, categoryId);
            }

            // Only published products
            filter &= filterBuilder.Eq(p => p.Status, ProductStatus.Published);

            var products = await context.Products
                .Find(filter)
                .SortByDescending(p => p.CreatedAt)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();

            var dtos = products.Select(MapToListDto).ToList();

            logger.OperationInfo(operation, $"Retrieved {dtos.Count} products");
            logger.OperationCompleted(operation, DateTimeOffset.UtcNow, sw.ElapsedMilliseconds);

            return Result<List<ProductListDto>>.Success(dtos);
        }
        catch (MongoException ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"MongoDB error: {ex.Message}");
            return Result<List<ProductListDto>>.Failure(
                ResultError.InternalServerError("Database error while retrieving products"));
        }
        catch (Exception ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"Unexpected error:  {ex.Message}");
            return Result<List<ProductListDto>>.Failure(
                ResultError.InternalServerError("Failed to retrieve products"));
        }
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    public async Task<Result<ProductDto>> GetByIdAsync(string id, Language language)
    {
        var sw = Stopwatch.StartNew();
        const string operation = nameof(GetByIdAsync);

        try
        {
            logger.OperationStarted(operation, DateTimeOffset.UtcNow);
            logger.OperationInfo(operation, $"Retrieving product: ID={id}, Language={language.GetCode()}");

            if (!ObjectId.TryParse(id, out _))
            {
                logger.OperationWarning(operation, $"Invalid ObjectId format: {id}");
                return Result<ProductDto>.Failure(
                    ResultError.BadRequest($"Invalid product ID format: {id}"));
            }

            var product = await context.Products
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                logger.OperationWarning(operation, $"Product not found: {id}");
                return Result<ProductDto>.Failure(
                    ResultError.NotFound($"Product with ID '{id}' not found"));
            }

            // Increment view count
            await IncrementViewCountAsync(id);

            var dto = await MapToDtoAsync(product, language);

            logger.OperationInfo(operation, $"Product retrieved:  {product.Title}");
            logger.OperationCompleted(operation, DateTimeOffset.UtcNow, sw.ElapsedMilliseconds);

            return Result<ProductDto>.Success(dto);
        }
        catch (MongoException ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"MongoDB error: {ex.Message}");
            return Result<ProductDto>.Failure(
                ResultError.InternalServerError("Database error while retrieving product"));
        }
        catch (Exception ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"Unexpected error: {ex.Message}");
            return Result<ProductDto>.Failure(
                ResultError.InternalServerError("Failed to retrieve product"));
        }
    }

    /// <summary>
    /// Create new product with files (images and videos)
    /// Product is immediately published
    /// </summary>
    public async Task<Result<ProductDto>> CreateAsync(CreateProductDto dto)
    {
        var sw = Stopwatch.StartNew();
        const string operation = nameof(CreateAsync);

        try
        {
            logger.OperationStarted(operation, DateTimeOffset.UtcNow);
            logger.OperationInfo(operation, $"Creating product: {dto.Title}");

            // ✅ 1. Validate category exists
            if (!ObjectId.TryParse(dto.CategoryId, out _))
            {
                logger.OperationWarning(operation, $"Invalid category ID:  {dto.CategoryId}");
                return Result<ProductDto>.Failure(
                    ResultError.BadRequest($"Invalid category ID format: {dto.CategoryId}"));
            }

            var categoryResult = await categoryService.GetByIdAsync(dto.CategoryId, Language.Russian);
            if (!categoryResult.IsSuccess)
            {
                logger.OperationWarning(operation, $"Category not found: {dto.CategoryId}");
                return Result<ProductDto>.Failure(
                    ResultError.NotFound($"Category with ID '{dto.CategoryId}' not found"));
            }

            // ✅ 2. Upload files (if provided)
            var uploadedFiles = new List<ProductFile>();

            if (dto.Files is { Count: > 0 })
            {
                logger.OperationInfo(operation, $"Uploading {dto.Files.Count} files");

                var displayOrder = 0;

                foreach (var file in dto.Files)
                {
                    try
                    {
                        // Determine file type
                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        var isImage = IsImageExtension(extension);
                        var isVideo = IsVideoExtension(extension);

                        if (!isImage && !isVideo)
                        {
                            logger.OperationWarning(operation, $"Unsupported file type: {extension}");
                            continue;
                        }

                        // Upload file
                        var uploadResult = isImage
                            ? await fileService.UploadImageAsync(file)
                            : await fileService.UploadVideoAsync(file);

                        if (!uploadResult.IsSuccess)
                        {
                            logger.OperationWarning(operation,
                                $"Failed to upload file '{file.FileName}': {uploadResult.Error.Message}");
                            continue;
                        }

                        var uploadedFile = uploadResult.Value!;

                        // Create ProductFile entity
                        uploadedFiles.Add(new ProductFile
                        {
                            FileName = uploadedFile.FileName,
                            FilePath = uploadedFile.FilePath,
                            FileSizeBytes = uploadedFile.FileSizeBytes,
                            MimeType = uploadedFile.MimeType,
                            FileType = uploadedFile.FileType,
                            DisplayOrder = displayOrder++,
                            UploadedAt = uploadedFile.UploadedAt
                        });

                        logger.OperationInfo(operation, $"File uploaded:  {uploadedFile.FileName}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to upload file: {FileName}", file.FileName);
                        // Continue with other files
                    }
                }

                logger.OperationInfo(operation, $"Successfully uploaded {uploadedFiles.Count} files");
            }

            // ✅ 3. Create product entity (immediately published)
            var product = new Product
            {
                CategoryId = dto.CategoryId,
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                Status = ProductStatus.Published, // ✅ Immediately published
                Files = uploadedFiles,
                DynamicFields = dto.DynamicFields ?? new Dictionary<string, string>(),
                Location = dto.Location,
                ContactPhone = dto.ContactPhone,
                ViewCount = 0,
                IsAiGenerated = dto.IsAiGenerated,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PublishedAt = DateTime.UtcNow
            };

            // ✅ 4. Save to database
            await context.Products.InsertOneAsync(product);

            var resultDto = await MapToDtoAsync(product, Language.Russian);

            logger.OperationInfo(operation,
                $"Product created successfully: ID={product.Id}, Files={uploadedFiles.Count}");
            logger.OperationCompleted(operation, DateTimeOffset.UtcNow, sw.ElapsedMilliseconds);

            return Result<ProductDto>.Success(resultDto);
        }
        catch (MongoException ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"MongoDB error: {ex.Message}");
            return Result<ProductDto>.Failure(
                ResultError.InternalServerError("Database error while creating product"));
        }
        catch (Exception ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"Unexpected error: {ex.Message}");
            return Result<ProductDto>.Failure(
                ResultError.InternalServerError("Failed to create product"));
        }
    }

    /// <summary>
    /// Delete product and all associated files
    /// </summary>
    public async Task<Result<bool>> DeleteAsync(string id)
    {
        var sw = Stopwatch.StartNew();
        const string operation = nameof(DeleteAsync);

        try
        {
            logger.OperationStarted(operation, DateTimeOffset.UtcNow);
            logger.OperationInfo(operation, $"Deleting product: {id}");

            if (!ObjectId.TryParse(id, out _))
            {
                logger.OperationWarning(operation, $"Invalid ObjectId format: {id}");
                return Result<bool>.Failure(
                    ResultError.BadRequest($"Invalid product ID format: {id}"));
            }

            // ✅ 1. Get product to delete files
            var product = await context.Products
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                logger.OperationWarning(operation, $"Product not found: {id}");
                return Result<bool>.Failure(
                    ResultError.NotFound($"Product with ID '{id}' not found"));
            }

            // ✅ 2. Delete all files
            if (product.Files.Count > 0)
            {
                logger.OperationInfo(operation, $"Deleting {product.Files.Count} files");

                foreach (var file in product.Files)
                {
                    try
                    {
                        var deleteResult = await fileService.DeleteFileAsync(file.FilePath);

                        if (deleteResult.IsSuccess)
                        {
                            logger.OperationInfo(operation, $"Deleted file: {file.FileName}");
                        }
                        else
                        {
                            logger.OperationWarning(operation,
                                $"Failed to delete file '{file.FileName}': {deleteResult.Error.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Error deleting file: {FileName}", file.FileName);
                        // Continue with other files
                    }
                }
            }

            // ✅ 3. Delete product from database
            await context.Products.DeleteOneAsync(p => p.Id == id);

            logger.OperationInfo(operation, $"Product deleted successfully:  {id}");
            logger.OperationCompleted(operation, DateTimeOffset.UtcNow, sw.ElapsedMilliseconds);

            return Result<bool>.Success(true);
        }
        catch (MongoException ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"MongoDB error: {ex.Message}");
            return Result<bool>.Failure(
                ResultError.InternalServerError("Database error while deleting product"));
        }
        catch (Exception ex)
        {
            logger.OperationException(ex, operation);
            logger.OperationFail(operation, $"Unexpected error:  {ex.Message}");
            return Result<bool>.Failure(
                ResultError.InternalServerError("Failed to delete product"));
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Map Product entity to ProductDto
    /// </summary>
    private async Task<ProductDto> MapToDtoAsync(Product product, Language language)
    {
        string? categoryName = null;

        try
        {
            var categoryResult = await categoryService.GetByIdAsync(product.CategoryId, language);
            if (categoryResult.IsSuccess)
            {
                categoryName = categoryResult.Value?.Name;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get category name for product {ProductId}", product.Id);
        }

        return new ProductDto
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            CategoryName = categoryName,
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            Status = product.Status,
            Files = product.Files.Select(f => new ProductFileDto
            {
                FileName = f.FileName,
                FilePath = f.FilePath,
                FileUrl = $"/{f.FilePath}",
                FileSizeBytes = f.FileSizeBytes,
                MimeType = f.MimeType,
                FileType = f.FileType,
                DisplayOrder = f.DisplayOrder,
                UploadedAt = f.UploadedAt
            }).ToList(),
            DynamicFields = product.DynamicFields,
            Location = product.Location,
            ContactPhone = product.ContactPhone,
            ViewCount = product.ViewCount,
            IsAiGenerated = product.IsAiGenerated,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            PublishedAt = product.PublishedAt
        };
    }

    /// <summary>
    /// Map Product entity to ProductListDto
    /// </summary>
    private static ProductListDto MapToListDto(Product product)
    {
        var firstImage = product.Files
            .Where(f => f.FileType == FileType.Image)
            .OrderBy(f => f.DisplayOrder)
            .FirstOrDefault();

        return new ProductListDto
        {
            Id = product.Id,
            Title = product.Title,
            Price = product.Price,
            Status = product.Status,
            ThumbnailUrl = firstImage != null ? $"/{firstImage.FilePath}" : null,
            Location = product.Location,
            ViewCount = product.ViewCount,
            CreatedAt = product.CreatedAt,
            PublishedAt = product.PublishedAt
        };
    }

    /// <summary>
    /// Increment product view count
    /// </summary>
    private async Task IncrementViewCountAsync(string productId)
    {
        try
        {
            var update = Builders<Product>.Update.Inc(p => p.ViewCount, 1);
            await context.Products.UpdateOneAsync(p => p.Id == productId, update);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to increment view count for product {ProductId}", productId);
            // Don't fail the request
        }
    }

    private static bool IsImageExtension(string extension)
    {
        var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ". webp", ".heic", ".heif", ".gif", ".bmp" };
        return imageExtensions.Contains(extension);
    }

    /// <summary>
    /// Check if extension is video
    /// </summary>
    private static bool IsVideoExtension(string extension)
    {
        var videoExtensions = new[] { ".mp4", ".mov", ".avi", ".webm", ".mkv", ".flv", ".wmv" };
        return videoExtensions.Contains(extension);
    }

    #endregion
}