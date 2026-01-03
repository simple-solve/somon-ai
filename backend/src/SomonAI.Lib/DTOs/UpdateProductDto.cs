namespace SomonAI.Lib.DTOs;

/// <summary>
/// DTO for updating an existing product
/// </summary>
public class UpdateProductDto
{
    public string?  Title { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public Dictionary<string, string>? DynamicFields { get; set; }
    public string? Location { get; set; }
    public string? ContactPhone { get; set; }
}