namespace SomonAI.Lib.Services;

/// <summary>
/// File service implementation with Result Pattern and structured logging
/// Handles image and video uploads with separate size limits
/// Uses IHostEnvironment for cross-platform compatibility
/// </summary>
public class FileService : IFileService
{
    private readonly FileStorageSettings _settings;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<FileService> _logger;
    private readonly string _uploadPath;

    public FileService(
        IOptions<FileStorageSettings> settings,
        IHostEnvironment environment,
        ILogger<FileService> logger)
    {
        _settings = settings.Value;
        _environment = environment;
        _logger = logger;

        // IHostEnvironment uses ContentRootPath instead of WebRootPath
        var wwwrootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
        _uploadPath = Path.Combine(wwwrootPath, _settings.UploadPath);

        // Ensure upload directories exist
        EnsureDirectoriesExist();
    }

    /// <summary>
    /// Upload image file
    /// </summary>
    public async Task<Result<FileUploadResult>> UploadImageAsync(IFormFile file)
    {
        var sw = Stopwatch.StartNew();
        const string operation = nameof(UploadImageAsync);

        try
        {
            _logger.OperationStarted(operation, DateTimeOffset.UtcNow);
            _logger.OperationInfo(operation, $"Uploading image:  {file.FileName}, Size: {file.Length / 1024.0:F2} KB");

            // Validate file
            var validationResult = ValidateFile(file, FileType.Image);
            if (!validationResult.IsSuccess)
            {
                _logger.OperationWarning(operation, validationResult.Error.Message ?? "Validation failed");
                return Result<FileUploadResult>.Failure(validationResult.Error);
            }

            // Upload to images folder
            var result = await UploadFileInternalAsync(file, "images", FileType.Image);

            if (result.IsSuccess)
            {
                _logger.OperationInfo(operation, $"Image uploaded successfully: {result.Value?.FileName}");
            }

            _logger.OperationCompleted(operation, DateTimeOffset.UtcNow, sw.ElapsedMilliseconds);
            return result;
        }
        catch (IOException ex)
        {
            _logger.OperationException(ex, operation);
            _logger.OperationFail(operation, $"IO error while uploading image: {ex.Message}");
            return Result<FileUploadResult>.Failure(
                ResultError.InternalServerError("Failed to save image file"));
        }
        catch (Exception ex)
        {
            _logger.OperationException(ex, operation);
            _logger.OperationFail(operation, $"Unexpected error:  {ex.Message}");
            return Result<FileUploadResult>.Failure(
                ResultError.InternalServerError("Failed to upload image"));
        }
    }

    /// <summary>
    /// Upload video file
    /// </summary>
    public async Task<Result<FileUploadResult>> UploadVideoAsync(IFormFile file)
    {
        var sw = Stopwatch.StartNew();
        const string operation = nameof(UploadVideoAsync);

        try
        {
            _logger.OperationStarted(operation, DateTimeOffset.UtcNow);
            _logger.OperationInfo(operation, $"Uploading video: {file.FileName}, Size: {file.Length / 1024.0 / 1024.0:F2} MB");

            // Validate file
            var validationResult = ValidateFile(file, FileType.Video);
            if (!validationResult.IsSuccess)
            {
                _logger.OperationWarning(operation, validationResult.Error.Message ?? "Validation failed");
                return Result<FileUploadResult>.Failure(validationResult.Error);
            }

            // Upload to videos folder
            var result = await UploadFileInternalAsync(file, "videos", FileType.Video);

            if (result.IsSuccess)
            {
                _logger.OperationInfo(operation, $"Video uploaded successfully: {result.Value?.FileName}");
            }

            _logger.OperationCompleted(operation, DateTimeOffset.UtcNow, sw.ElapsedMilliseconds);
            return result;
        }
        catch (IOException ex)
        {
            _logger.OperationException(ex, operation);
            _logger.OperationFail(operation, $"IO error while uploading video: {ex.Message}");
            return Result<FileUploadResult>.Failure(
                ResultError.InternalServerError("Failed to save video file"));
        }
        catch (Exception ex)
        {
            _logger.OperationException(ex, operation);
            _logger.OperationFail(operation, $"Unexpected error: {ex.Message}");
            return Result<FileUploadResult>.Failure(
                ResultError.InternalServerError("Failed to upload video"));
        }
    }

    /// <summary>
    /// Upload file (auto-detect type)
    /// </summary>
    public async Task<Result<FileUploadResult>> UploadFileAsync(IFormFile file)
    {
        var sw = Stopwatch.StartNew();
        const string operation = nameof(UploadFileAsync);

        try
        {
            _logger.OperationStarted(operation, DateTimeOffset.UtcNow);
            _logger.OperationInfo(operation, $"Auto-detecting file type for: {file.FileName}");

            // Detect file type
            var fileType = DetectFileType(file);
            if (fileType == null)
            {
                var extension = Path.GetExtension(file.FileName);
                _logger.OperationWarning(operation, $"Unsupported file type: {extension}");
                return Result<FileUploadResult>.Failure(
                    ResultError.UnsupportedMediaType($"Unsupported file type:  {extension}"));
            }

            _logger.OperationInfo(operation, $"Detected file type: {fileType}");

            // Upload based on type
            var result = fileType == FileType.Image
                ? await UploadImageAsync(file)
                : await UploadVideoAsync(file);

            _logger.OperationCompleted(operation, DateTimeOffset.UtcNow, sw.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.OperationException(ex, operation);
            _logger.OperationFail(operation, $"Failed to upload file: {ex.Message}");
            return Result<FileUploadResult>.Failure(
                ResultError.InternalServerError("Failed to upload file"));
        }
    }

    /// <summary>
    /// Delete file by path
    /// </summary>
    public async Task<Result<bool>> DeleteFileAsync(string filePath)
    {
        var sw = Stopwatch.StartNew();
        const string operation = nameof(DeleteFileAsync);

        try
        {
            _logger.OperationStarted(operation, DateTimeOffset.UtcNow);
            _logger.OperationInfo(operation, $"Deleting file: {filePath}");

            if (string.IsNullOrWhiteSpace(filePath))
            {
                _logger.OperationWarning(operation, "File path is null or empty");
                return Result<bool>.Failure(ResultError.BadRequest("File path cannot be empty"));
            }

            var wwwrootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            var fullPath = Path.Combine(wwwrootPath, filePath.TrimStart('/'));

            if (!File.Exists(fullPath))
            {
                _logger.OperationWarning(operation, $"File not found: {fullPath}");
                return Result<bool>.Failure(ResultError.NotFound($"File not found: {filePath}"));
            }

            // Delete file
            await Task.Run(() => File.Delete(fullPath));

            _logger.OperationInfo(operation, $"File deleted successfully: {filePath}");
            _logger.OperationCompleted(operation, DateTimeOffset.UtcNow, sw.ElapsedMilliseconds);

            return Result<bool>.Success(true);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.OperationException(ex, operation);
            _logger.OperationFail(operation, $"Access denied:  {ex.Message}");
            return Result<bool>.Failure(ResultError.AccessDenied("Access denied to delete file"));
        }
        catch (IOException ex)
        {
            _logger.OperationException(ex, operation);
            _logger.OperationFail(operation, $"IO error:  {ex.Message}");
            return Result<bool>.Failure(ResultError.InternalServerError("File is in use or locked"));
        }
        catch (Exception ex)
        {
            _logger.OperationException(ex, operation);
            _logger.OperationFail(operation, $"Failed to delete file: {ex.Message}");
            return Result<bool>.Failure(ResultError.InternalServerError("Failed to delete file"));
        }
    }

    /// <summary>
    /// Get file stream for download
    /// </summary>
    public Task<Result<FileStreamResult>> GetFileAsync(string filePath)
    {
        var sw = Stopwatch.StartNew();
        const string operation = nameof(GetFileAsync);

        try
        {
            _logger.OperationStarted(operation, DateTimeOffset.UtcNow);
            _logger.OperationInfo(operation, $"Getting file:  {filePath}");

            if (string.IsNullOrWhiteSpace(filePath))
            {
                _logger.OperationWarning(operation, "File path is null or empty");
                return Task.FromResult(Result<FileStreamResult>.Failure(ResultError.BadRequest("File path cannot be empty")));
            }

            var wwwrootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            var fullPath = Path.Combine(wwwrootPath, filePath.TrimStart('/'));

            if (!File.Exists(fullPath))
            {
                _logger.OperationWarning(operation, $"File not found: {fullPath}");
                return Task.FromResult(Result<FileStreamResult>.Failure(ResultError.NotFound($"File not found: {filePath}")));
            }

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var contentType = GetContentType(fullPath);
            var fileName = Path.GetFileName(fullPath);

            var result = new FileStreamResult
            {
                FileStream = stream,
                ContentType = contentType,
                FileName = fileName
            };

            _logger.OperationInfo(operation, $"File retrieved:  {fileName}, Size: {stream.Length / 1024.0:F2} KB");
            _logger.OperationCompleted(operation, DateTimeOffset.UtcNow, sw.ElapsedMilliseconds);

            return Task.FromResult(Result<FileStreamResult>.Success(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.OperationException(ex, operation);
            _logger.OperationFail(operation, $"Access denied: {ex.Message}");
            return Task.FromResult(Result<FileStreamResult>.Failure(ResultError.AccessDenied("Access denied to read file")));
        }
        catch (IOException ex)
        {
            _logger.OperationException(ex, operation);
            _logger.OperationFail(operation, $"IO error:  {ex.Message}");
            return Task.FromResult(Result<FileStreamResult>.Failure(ResultError.InternalServerError("Failed to read file")));
        }
        catch (Exception ex)
        {
            _logger.OperationException(ex, operation);
            _logger.OperationFail(operation, $"Failed to get file: {ex.Message}");
            return Task.FromResult(Result<FileStreamResult>.Failure(
                ResultError.InternalServerError("Failed to retrieve file")));
        }
    }

    /// <summary>
    /// Check if file exists
    /// </summary>
    public async Task<Result<bool>> FileExistsAsync(string filePath)
    {
        var sw = Stopwatch.StartNew();
        const string operation = nameof(FileExistsAsync);

        try
        {
            _logger.OperationStarted(operation, DateTimeOffset.UtcNow);

            if (string.IsNullOrWhiteSpace(filePath))
            {
                _logger.OperationWarning(operation, "File path is null or empty");
                return Result<bool>.Success();
            }

            var wwwrootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            var fullPath = Path.Combine(wwwrootPath, filePath.TrimStart('/'));
            var exists = await Task.Run(() => File.Exists(fullPath));

            _logger.OperationInfo(operation, $"File exists check: {filePath} = {exists}");
            _logger.OperationCompleted(operation, DateTimeOffset.UtcNow, sw.ElapsedMilliseconds);

            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.OperationException(ex, operation);
            _logger.OperationFail(operation, $"Failed to check file existence: {ex.Message}");
            return Result<bool>.Failure(ResultError.InternalServerError("Failed to check file existence"));
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Internal file upload implementation
    /// </summary>
    private async Task<Result<FileUploadResult>> UploadFileInternalAsync(
        IFormFile file,
        string subfolder,
        FileType fileType)
    {
        try
        {
            // Generate unique filename
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            // Full path
            var uploadFolder = Path.Combine(_uploadPath, subfolder);
            var fullPath = Path.Combine(uploadFolder, uniqueFileName);

            // Relative path for URL
            var relativePath = $"{_settings.UploadPath}/{subfolder}/{uniqueFileName}";

            // Save file
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.OperationInfo(nameof(UploadFileInternalAsync),
                $"File saved:  {relativePath}, Size: {file.Length / 1024.0:F2} KB");

            var result = new FileUploadResult
            {
                FileName = uniqueFileName,
                FilePath = relativePath,
                FileUrl = $"/{relativePath}",
                FileSizeBytes = file.Length,
                MimeType = file.ContentType,
                FileType = fileType,
                UploadedAt = DateTime.UtcNow
            };

            return Result<FileUploadResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.OperationException(ex, nameof(UploadFileInternalAsync));
            return Result<FileUploadResult>.Failure(
                ResultError.InternalServerError("Failed to save file to disk"));
        }
    }

    /// <summary>
    /// Validate uploaded file with type-specific size limits
    /// </summary>
    private BaseResult ValidateFile(IFormFile? file, FileType expectedType)
    {
        // Check if file is null or empty
        if (file == null || file.Length == 0)
        {
            return BaseResult.Failure(ResultError.BadRequest("File is empty"));
        }

        // Check file size based on type
        var maxSizeBytes = expectedType == FileType.Image
            ? _settings.MaxImageSizeBytes
            : _settings.MaxVideoSizeBytes;

        var maxSizeMb = expectedType == FileType.Image
            ? _settings.MaxImageSizeMb
            : _settings.MaxVideoSizeMb;

        if (file.Length > maxSizeBytes)
        {
            var fileTypeName = expectedType == FileType.Image ? "Image" : "Video";
            var actualSizeMb = file.Length / 1024.0 / 1024.0;

            return BaseResult.Failure(
                ResultError.BadRequest(
                    $"{fileTypeName} size ({actualSizeMb:F2} MB) exceeds maximum allowed size of {maxSizeMb} MB"));
        }

        // Check extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExtensions = expectedType == FileType.Image
            ? _settings.AllowedImageExtensions
            : _settings.AllowedVideoExtensions;

        if (!allowedExtensions.Contains(extension))
        {
            var fileTypeName = expectedType == FileType.Image ? "image" : "video";
            return BaseResult.Failure(
                ResultError.UnsupportedMediaType(
                    $"File type '{extension}' is not allowed for {fileTypeName}.  Allowed:  {string.Join(", ", allowedExtensions)}"));
        }

        return BaseResult.Success();
    }

    /// <summary>
    /// Detect file type from extension
    /// </summary>
    private FileType? DetectFileType(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (_settings.AllowedImageExtensions.Contains(extension))
            return FileType.Image;

        if (_settings.AllowedVideoExtensions.Contains(extension))
            return FileType.Video;

        return null;
    }

    /// <summary>
    /// Get MIME content type based on file extension
    /// </summary>
    private static string GetContentType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            // Images
            ".jpg" or ".jpeg" => "image/jpeg",
            ". png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".heic" => "image/heic",
            ".heif" => "image/heif",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",

            // Videos
            ".mp4" => "video/mp4",
            ".mov" => "video/quicktime",
            ".avi" => "video/x-msvideo",
            ". webm" => "video/webm",
            ".mkv" => "video/x-matroska",
            ".flv" => "video/x-flv",
            ".wmv" => "video/x-ms-wmv",

            // Default
            _ => "application/octet-stream"
        };
    }

    /// <summary>
    /// Ensure upload directories exist on service initialization
    /// </summary>
    private void EnsureDirectoriesExist()
    {
        try
        {
            var imagesPath = Path.Combine(_uploadPath, "images");
            var videosPath = Path.Combine(_uploadPath, "videos");

            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
                _logger.LogInformation("✅ Created images directory:  {Path}", imagesPath);
            }

            if (!Directory.Exists(videosPath))
            {
                Directory.CreateDirectory(videosPath);
                _logger.LogInformation("✅ Created videos directory: {Path}", videosPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to create upload directories");
            throw;
        }
    }

    #endregion
}