namespace SomonAI.Lib.DataAccess;

/// <summary>
/// MongoDB database context interface
/// </summary>
public interface IMongoDbContext
{
    /// <summary>
    /// Categories collection
    /// </summary>
    IMongoCollection<Category> Categories { get; }

    /// <summary>
    /// Products collection
    /// </summary>
    IMongoCollection<Product> Products { get; }

    /// <summary>
    /// Get MongoDB database instance
    /// </summary>
    IMongoDatabase Database { get; }
}