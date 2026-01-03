namespace SomonAI.Lib.Localization;

/// <summary>
/// Localization services registration
/// </summary>
public static class LocalizationServiceExtensions
{
    public static IServiceCollection AddLocalizationServices(this IServiceCollection services)
    {
        services.AddScoped<ILanguageProvider, LanguageProvider>();

        return services;
    }
}