namespace SomonAI.Lib.DataAccess.Entities;

/// <summary>
/// Represents a product listing with dynamic fields and multiple files
/// </summary>
public sealed class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Reference to Category ID
    /// </summary>
    [BsonElement("categoryId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CategoryId { get; set; } = null!;

    /// <summary>
    /// Product title/name
    /// </summary>
    [BsonElement("title")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// Detailed description (can be AI-generated)
    /// </summary>
    [BsonElement("description")]
    public string Description { get; set; } = null!;

    /// <summary>
    /// Price in TJS (Tajik Somoni)
    /// </summary>
    [BsonElement("price")]
    public decimal Price { get; set; }

    /// <summary>
    /// Product status
    /// </summary>
    [BsonElement("status")]
    public ProductStatus Status { get; set; } = ProductStatus.Draft;

    /// <summary>
    /// List of images and videos
    /// </summary>
    [BsonElement("files")]
    public List<ProductFile> Files { get; set; } = new();

    /// <summary>
    /// Dynamic fields specific to category (e.g., for cars:  brand, model, year)
    /// Stored as key-value pairs for flexibility
    /// </summary>
    [BsonElement("dynamicFields")]
    public Dictionary<string, string> DynamicFields { get; set; } = new();

    /// <summary>
    /// Location/city
    /// </summary>
    [BsonElement("location")]
    public string? Location { get; set; }

    /// <summary>
    /// Contact phone number
    /// </summary>
    [BsonElement("contactPhone")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// View count
    /// </summary>
    [BsonElement("viewCount")]
    public int ViewCount { get; set; } = 0;

    /// <summary>
    /// Whether AI was used to generate description
    /// </summary>
    [BsonElement("isAiGenerated")]
    public bool IsAiGenerated { get; set; } = false;

    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("publishedAt")] public DateTime? PublishedAt { get; set; }
}