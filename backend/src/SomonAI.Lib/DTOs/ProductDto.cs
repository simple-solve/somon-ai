namespace SomonAI.Lib.DTOs;

/// <summary>
/// Product DTO for API responses
/// </summary>
public class ProductDto
{
    public string Id { get; set; } = null!;
    public string CategoryId { get; set; } = null!;
    
    /// <summary>
    /// Category name (localized)
    /// </summary>
    public string?  CategoryName { get; set; }
    
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null! ;
    public decimal Price { get; set; }
    public ProductStatus Status { get; set; }
    
    /// <summary>
    /// List of image and video files
    /// </summary>
    public List<ProductFileDto> Files { get; set; } = new();
    
    /// <summary>
    /// Dynamic fields (e.g., brand, model, year for cars)
    /// </summary>
    public Dictionary<string, string> DynamicFields { get; set; } = new();
    
    public string? Location { get; set; }
    public string? ContactPhone { get; set; }
    public int ViewCount { get; set; }
    public bool IsAiGenerated { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}