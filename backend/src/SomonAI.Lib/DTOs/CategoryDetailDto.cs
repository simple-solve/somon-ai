namespace SomonAI.Lib.DTOs;

/// <summary>
/// Detailed category DTO with all 3 language variants
/// </summary>
public class CategoryDetailDto
{
    public string Id { get; set; } = null!;
    public string Slug { get; set; } = null!;
    
    public string NameRu { get; set; } = null!;
    public string NameTj { get; set; } = null!;
    public string NameEn { get; set; } = null! ;
    
    public string?  DescriptionRu { get; set; }
    public string?  DescriptionTj { get; set; }
    public string?  DescriptionEn { get; set; }
    
    public string?  Icon { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}