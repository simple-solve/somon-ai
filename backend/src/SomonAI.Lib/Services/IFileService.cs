namespace SomonAI.Lib.Services;

/// <summary>
/// File service interface for handling file uploads, downloads and deletions
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Upload image file
    /// </summary>
    Task<Result<FileUploadResult>> UploadImageAsync(IFormFile file);

    /// <summary>
    /// Upload video file
    /// </summary>
    Task<Result<FileUploadResult>> UploadVideoAsync(IFormFile file);

    /// <summary>
    /// Upload file (auto-detect type)
    /// </summary>
    Task<Result<FileUploadResult>> UploadFileAsync(IFormFile file);

    /// <summary>
    /// Delete file by path
    /// </summary>
    Task<Result<bool>> DeleteFileAsync(string filePath);

    /// <summary>
    /// Get file stream for download
    /// </summary>
    Task<Result<FileStreamResult>> GetFileAsync(string filePath);

    /// <summary>
    /// Check if file exists
    /// </summary>
    Task<Result<bool>> FileExistsAsync(string filePath);
}

/// <summary>
/// File stream result for download
/// </summary>
public class FileStreamResult
{
    public Stream FileStream { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string FileName { get; set; } = null!;
}


public class FileUploadResult
{
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public long FileSizeBytes { get; set; }
    public string MimeType { get; set; } = null!;
    public FileType FileType { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}