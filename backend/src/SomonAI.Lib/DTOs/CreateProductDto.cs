namespace SomonAI.Lib.DTOs;

/// <summary>
/// DTO for creating a new product with files
/// </summary>
public class CreateProductDto
{
    public string CategoryId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null! ;
    public decimal Price { get; set; }
    public Dictionary<string, string>? DynamicFields { get; set; }
    public string?  Location { get; set; }
    public string? ContactPhone { get; set; }
    public bool IsAiGenerated { get; set; }
    
    public List<IFormFile>? Files { get; set; }
}