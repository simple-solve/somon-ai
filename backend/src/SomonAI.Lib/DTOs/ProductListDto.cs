namespace SomonAI.Lib.DTOs;

/// <summary>
/// Simplified product DTO for list views
/// </summary>
public class ProductListDto
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }
    public ProductStatus Status { get; set; }
    
    /// <summary>
    /// First image thumbnail
    /// </summary>
    public string? ThumbnailUrl { get; set; }
    
    public string? Location { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime?  PublishedAt { get; set; }
}