namespace SomonAI.API.Infrastructure.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(
            configuration.GetSection(MongoDbSettings.SectionName));
        services.AddSingleton<IMongoDbContext, MongoDbContext>();
        services.AddScoped<DbInitializer>();

        services.Configure<FileStorageSettings>(
            configuration.GetSection(FileStorageSettings.SectionName));

        services.AddScoped<ILanguageProvider, LanguageProvider>();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IFileService, FileService>();

        return services;
    }
}