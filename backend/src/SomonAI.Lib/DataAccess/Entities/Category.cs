namespace SomonAI.Lib.DataAccess.Entities;

/// <summary>
/// Represents a product category with multilingual support (Russian + Tajik)
/// </summary>
public sealed class Category
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Unique slug for URL-friendly category identification
    /// </summary>
    [BsonElement("slug")]
    public string Slug { get; set; } = null!;

    /// <summary>
    /// Category name in Russian
    /// </summary>
    [BsonElement("nameRu")]
    public string NameRu { get; set; } = null!;

    /// <summary>
    /// Category name in Tajik
    /// </summary>
    [BsonElement("nameTj")]
    public string NameTj { get; set; } = null!;

    /// <summary>
    /// Category description in Russian
    /// </summary>
    [BsonElement("descriptionRu")]
    public string? DescriptionRu { get; set; }

    /// <summary>
    /// Category description in Tajik
    /// </summary>
    [BsonElement("descriptionTj")]
    public string? DescriptionTj { get; set; }

    /// <summary>
    /// Icon or emoji for the category
    /// </summary>
    [BsonElement("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// Display order for sorting
    /// </summary>
    [BsonElement("displayOrder")]
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Whether the category is active
    /// </summary>
    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}