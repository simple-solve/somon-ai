namespace SomonAI.Lib.DataAccess.Entities;

/// <summary>
/// Represents a file (image or video) associated with a product
/// </summary>
public sealed class ProductFile
{
    /// <summary>
    /// File name with extension
    /// </summary>
    [BsonElement("fileName")]
    public string FileName { get; set; } = null!;

    /// <summary>
    /// Relative path from wwwroot (e.g., "uploads/images/abc123.jpg")
    /// </summary>
    [BsonElement("filePath")]
    public string FilePath { get; set; } = null!;

    /// <summary>
    /// File size in bytes
    /// </summary>
    [BsonElement("fileSizeBytes")]
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// MIME type (e.g., "image/jpeg", "video/mp4")
    /// </summary>
    [BsonElement("mimeType")]
    public string MimeType { get; set; } = null!;

    /// <summary>
    /// Type of file (Image or Video)
    /// </summary>
    [BsonElement("fileType")]
    public FileType FileType { get; set; }

    /// <summary>
    /// Display order in product gallery
    /// </summary>
    [BsonElement("displayOrder")]
    public int DisplayOrder { get; set; }

    /// <summary>
    /// When the file was uploaded
    /// </summary>
    [BsonElement("uploadedAt")]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}