using SomonAI.Lib.Configuration;

namespace SomonAI.Lib.DataAccess;

/// <summary>
/// MongoDB database context implementation
/// </summary>
public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly MongoDbSettings _settings;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        _settings = settings.Value;

        var client = new MongoClient(_settings.ConnectionString);
        _database = client.GetDatabase(_settings.DatabaseName);

        // Ensure indexes are created
        CreateIndexes();
    }

    public IMongoCollection<Category> Categories =>
        _database.GetCollection<Category>(_settings.CategoriesCollection);

    public IMongoCollection<Product> Products =>
        _database.GetCollection<Product>(_settings.ProductsCollection);

    public IMongoDatabase Database => _database;

    /// <summary>
    /// Create database indexes for better performance
    /// </summary>
    private void CreateIndexes()
    {
        // Category indexes
        var categoryIndexKeys = Builders<Category>.IndexKeys
            .Ascending(c => c.Slug)
            .Ascending(c => c.IsActive);

        var categoryIndexModel = new CreateIndexModel<Category>(
            categoryIndexKeys,
            new CreateIndexOptions { Name = "idx_slug_isactive" }
        );

        Categories.Indexes.CreateOne(categoryIndexModel);

        // Product indexes
        var productCategoryIndexKeys = Builders<Product>.IndexKeys
            .Ascending(p => p.CategoryId)
            .Descending(p => p.CreatedAt);

        var productCategoryIndexModel = new CreateIndexModel<Product>(
            productCategoryIndexKeys,
            new CreateIndexOptions { Name = "idx_categoryid_createdat" }
        );

        var productStatusIndexKeys = Builders<Product>.IndexKeys
            .Ascending(p => p.Status)
            .Descending(p => p.PublishedAt);

        var productStatusIndexModel = new CreateIndexModel<Product>(
            productStatusIndexKeys,
            new CreateIndexOptions { Name = "idx_status_publishedat" }
        );

        Products.Indexes.CreateMany(new[]
        {
            productCategoryIndexModel,
            productStatusIndexModel
        });
    }
}