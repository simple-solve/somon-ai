namespace SomonAI.Lib.DTOs;

/// <summary>
/// Product file DTO
/// </summary>
public class ProductFileDto
{
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    
    /// <summary>
    /// Full URL to access the file
    /// </summary>
    public string FileUrl { get; set; } = null!;
    
    public long FileSizeBytes { get; set; }
    public string MimeType { get; set; } = null!;
    public FileType FileType { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime UploadedAt { get; set; }
}