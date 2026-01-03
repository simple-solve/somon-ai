namespace SomonAI.Lib.DataAccess;

/// <summary>
/// Extension methods for database initialization
/// </summary>
public static class DatabaseInitializerExtensions
{
    /// <summary>
    /// Initialize database with collections, indexes and seed data
    /// </summary>
    public static async Task<IServiceProvider> InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        DbInitializer initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();

        await initializer.InitializeAsync();

        return serviceProvider;
    }
}