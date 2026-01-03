namespace SomonAI.Lib.DataAccess;

/// <summary>
/// Database initializer - creates database, collections, indexes and seeds initial data
/// </summary>
public class DbInitializer(IMongoDbContext context, ILogger<DbInitializer> logger)
{
    /// <summary>
    /// Initialize database with all required setup
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            logger.OperationStarted(nameof(InitializeAsync), DateTimeOffset.UtcNow);

            // 1. Create database if not exists
            await CreateDatabaseAsync();

            // 2. Create collections if not exist
            await CreateCollectionsAsync();

            // 3. Create indexes
            await CreateIndexesAsync();

            // 4. Seed initial data
            await SeedDataAsync();

            logger.OperationCompleted(nameof(InitializeAsync), DateTimeOffset.UtcNow, 0);
        }
        catch (Exception ex)
        {
            logger.OperationException(ex, nameof(InitializeAsync));
            throw;
        }
    }

    /// <summary>
    /// Create database if it doesn't exist
    /// </summary>
    private async Task CreateDatabaseAsync()
    {
        try
        {
            var client = context.Database.Client;
            var databaseName = context.Database.DatabaseNamespace.DatabaseName;

            // List all databases
            var databases = await client.ListDatabaseNamesAsync();
            var databaseList = await databases.ToListAsync();

            if (databaseList.Contains(databaseName))
            {
                logger.OperationInfo(nameof(CreateDatabaseAsync),
                    $"Database '{databaseName}' already exists");
            }
            else
            {
                // Creating first collection will create the database
                logger.OperationInfo(nameof(CreateDatabaseAsync),
                    $"Database '{databaseName}' will be created with first collection");
            }
        }
        catch (Exception ex)
        {
            logger.OperationException(ex, nameof(CreateDatabaseAsync));
            throw;
        }
    }

    /// <summary>
    /// Create collections if they don't exist
    /// </summary>
    private async Task CreateCollectionsAsync()
    {
        try
        {
            var database = context.Database;
            var existingCollections = await (await database.ListCollectionNamesAsync()).ToListAsync();

            // Create Categories collection
            if (!existingCollections.Contains("categories"))
            {
                await database.CreateCollectionAsync("categories");
                logger.OperationInfo(nameof(CreateCollectionsAsync),
                    "Created 'categories' collection");
            }
            else
            {
                logger.OperationInfo(nameof(CreateCollectionsAsync),
                    "'categories' collection already exists");
            }

            // Create Products collection
            if (!existingCollections.Contains("products"))
            {
                await database.CreateCollectionAsync("products");
                logger.OperationInfo(nameof(CreateCollectionsAsync),
                    "Created 'products' collection");
            }
            else
            {
                logger.OperationInfo(nameof(CreateCollectionsAsync),
                    "'products' collection already exists");
            }
        }
        catch (Exception ex)
        {
            logger.OperationException(ex, nameof(CreateCollectionsAsync));
            throw;
        }
    }

    /// <summary>
    /// Create indexes for better query performance
    /// </summary>
    private async Task CreateIndexesAsync()
    {
        try
        {
            logger.OperationInfo(nameof(CreateIndexesAsync), "Creating indexes...");

            // Categories indexes
            await CreateCategoryIndexesAsync();

            // Products indexes
            await CreateProductIndexesAsync();

            logger.OperationInfo(nameof(CreateIndexesAsync), "Indexes created successfully");
        }
        catch (Exception ex)
        {
            logger.OperationException(ex, nameof(CreateIndexesAsync));
            throw;
        }
    }

    /// <summary>
    /// Create indexes for Categories collection
    /// </summary>
    private async Task CreateCategoryIndexesAsync()
    {
        var collection = context.Categories;

        // Index 1: Slug (unique)
        var slugIndexKeys = Builders<Category>.IndexKeys.Ascending(c => c.Slug);
        var slugIndexModel = new CreateIndexModel<Category>(
            slugIndexKeys,
            new CreateIndexOptions
            {
                Name = "idx_slug_unique",
                Unique = true
            }
        );

        // Index 2: IsActive + DisplayOrder (for filtering active categories)
        var activeIndexKeys = Builders<Category>.IndexKeys
            .Ascending(c => c.IsActive)
            .Ascending(c => c.DisplayOrder);
        var activeIndexModel = new CreateIndexModel<Category>(
            activeIndexKeys,
            new CreateIndexOptions { Name = "idx_isactive_displayorder" }
        );

        await collection.Indexes.CreateManyAsync(new[]
        {
            slugIndexModel,
            activeIndexModel
        });

        logger.OperationInfo(nameof(CreateCategoryIndexesAsync),
            "Created 2 indexes for categories");
    }

    /// <summary>
    /// Create indexes for Products collection
    /// </summary>
    private async Task CreateProductIndexesAsync()
    {
        var collection = context.Products;

        // Index 1: CategoryId + CreatedAt (for category filtering)
        var categoryIndexKeys = Builders<Product>.IndexKeys
            .Ascending(p => p.CategoryId)
            .Descending(p => p.CreatedAt);
        var categoryIndexModel = new CreateIndexModel<Product>(
            categoryIndexKeys,
            new CreateIndexOptions { Name = "idx_categoryid_createdat" }
        );

        // Index 2: Status + PublishedAt (for published products)
        var statusIndexKeys = Builders<Product>.IndexKeys
            .Ascending(p => p.Status)
            .Descending(p => p.PublishedAt);
        var statusIndexModel = new CreateIndexModel<Product>(
            statusIndexKeys,
            new CreateIndexOptions { Name = "idx_status_publishedat" }
        );

        // Index 3: Title (text search)
        var titleIndexKeys = Builders<Product>.IndexKeys.Text(p => p.Title);
        var titleIndexModel = new CreateIndexModel<Product>(
            titleIndexKeys,
            new CreateIndexOptions { Name = "idx_title_text" }
        );

        // Index 4: CreatedAt (for sorting by date)
        var createdAtIndexKeys = Builders<Product>.IndexKeys.Descending(p => p.CreatedAt);
        var createdAtIndexModel = new CreateIndexModel<Product>(
            createdAtIndexKeys,
            new CreateIndexOptions { Name = "idx_createdat_desc" }
        );

        await collection.Indexes.CreateManyAsync(new[]
        {
            categoryIndexModel,
            statusIndexModel,
            titleIndexModel,
            createdAtIndexModel
        });

        logger.OperationInfo(nameof(CreateProductIndexesAsync),
            "Created 4 indexes for products");
    }

    /// <summary>
    /// Seed initial data
    /// </summary>
    private async Task SeedDataAsync()
    {
        await SeedCategoriesAsync();
        // Можно добавить seed для тестовых продуктов если нужно
        // await SeedTestProductsAsync();
    }
// ... (предыдущие методы остаются без изменений)

    /// <summary>
    /// Seed categories with trilingual data (RU, TJ, EN)
    /// </summary>
    private async Task SeedCategoriesAsync()
    {
        var count = await context.Categories.CountDocumentsAsync(_ => true);

        if (count > 0)
        {
            logger.OperationInfo(nameof(SeedCategoriesAsync),
                $"Categories already exist ({count} records). Skipping seed.");
            return;
        }

        logger.OperationInfo(nameof(SeedCategoriesAsync), "Seeding categories...");

        var categories = new List<Category>
        {
            new()
            {
                Slug = "real-estate",
                NameRu = "Недвижимость",
                NameTj = "Молу мулк",
                NameEn = "Real Estate",
                DescriptionRu = "Квартиры, дома, коммерческая недвижимость, аренда",
                DescriptionTj = "Хонаҳо, биноҳо, амволи тиҷоратӣ, иҷора",
                DescriptionEn = "Apartments, houses, commercial real estate, rent",
                Icon = "🏠",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Slug = "cars",
                NameRu = "Автомобили",
                NameTj = "Автомобилҳо",
                NameEn = "Cars",
                DescriptionRu = "Легковые автомобили, внедорожники, грузовики, мотоциклы",
                DescriptionTj = "Автомобилҳои сабук, автомобилҳои ҷангалӣ, мошинҳои боркаш, мотоциклҳо",
                DescriptionEn = "Passenger cars, SUVs, trucks, motorcycles",
                Icon = "🚗",
                DisplayOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Slug = "general-goods",
                NameRu = "Общие товары",
                NameTj = "Молҳои умумӣ",
                NameEn = "General Goods",
                DescriptionRu = "Электроника, мебель, одежда, техника и другие товары",
                DescriptionTj = "Электроника, мебел, либос, техника ва молҳои дигар",
                DescriptionEn = "Electronics, furniture, clothing, appliances and other goods",
                Icon = "📦",
                DisplayOrder = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Categories.InsertManyAsync(categories);

        logger.OperationInfo(nameof(SeedCategoriesAsync),
            $"Successfully seeded {categories.Count} categories with 3 languages (ru/tj/en)");
    }
}