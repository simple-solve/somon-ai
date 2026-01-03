namespace SomonAI.Lib.Configuration;

/// <summary>
/// File storage configuration settings
/// </summary>
public class FileStorageSettings
{
    public const string SectionName = "FileStorageSettings";

    /// <summary>
    /// Upload directory path (relative to wwwroot)
    /// </summary>
    public string UploadPath { get; set; } = "uploads";

    /// <summary>
    /// Maximum image file size in megabytes
    /// </summary>
    public int MaxImageSizeMb { get; set; } = 10;

    /// <summary>
    /// Maximum video file size in megabytes
    /// </summary>
    public int MaxVideoSizeMb { get; set; } = 100;

    /// <summary>
    /// Allowed image file extensions
    /// </summary>
    public List<string> AllowedImageExtensions { get; set; } = [];

    /// <summary>
    /// Allowed video file extensions
    /// </summary>
    public List<string> AllowedVideoExtensions { get; set; } = [];

    /// <summary>
    /// Get max image file size in bytes
    /// </summary>
    public long MaxImageSizeBytes => MaxImageSizeMb * 1024 * 1024;

    /// <summary>
    /// Get max video file size in bytes
    /// </summary>
    public long MaxVideoSizeBytes => MaxVideoSizeMb * 1024 * 1024;

    /// <summary>
    /// Get all allowed extensions
    /// </summary>
    public List<string> GetAllAllowedExtensions()
    {
        return AllowedImageExtensions.Concat(AllowedVideoExtensions).ToList();
    }
}