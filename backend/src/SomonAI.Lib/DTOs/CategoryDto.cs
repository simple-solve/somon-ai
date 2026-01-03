namespace SomonAI.Lib.DTOs;

/// <summary>
/// Category DTO with localized name based on current language
/// </summary>
public class CategoryDto
{
    public string Id { get; set; } = null!;
    public string Slug { get; set; } = null!;
    
    /// <summary>
    /// Localized name (based on Accept-Language)
    /// </summary>
    public string Name { get; set; } = null! ;
    
    /// <summary>
    /// Localized description
    /// </summary>
    public string?  Description { get; set; }
    
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}