namespace SomonAI.Lib.Configuration;

/// <summary>
/// MongoDB connection settings from appsettings.json
/// </summary>
public class MongoDbSettings
{
    public const string SectionName = "MongoDbSettings";

    /// <summary>
    /// MongoDB connection string
    /// Example: "mongodb://localhost:27017"
    /// </summary>
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// Database name
    /// </summary>
    public string DatabaseName { get; set; } = null!;

    /// <summary>
    /// Categories collection name
    /// </summary>
    public string CategoriesCollection { get; set; } = "categories";

    /// <summary>
    /// Products collection name
    /// </summary>
    public string ProductsCollection { get; set; } = "products";
}