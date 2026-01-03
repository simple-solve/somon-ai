namespace SomonAI.Lib.Localization;

/// <summary>
/// Extension methods for localization
/// </summary>
public static class LocalizationExtensions
{
    /// <summary>
    /// Get localized text based on current language
    /// </summary>
    public static string GetLocalized(this LocalizedString?  localizedString, ILanguageProvider languageProvider)
    {
        if (localizedString == null)
            return string.Empty;

        var currentLanguage = languageProvider.GetCurrentLanguage();
        return localizedString.Get(currentLanguage);
    }

    /// <summary>
    /// Get localized name from entity (for Category)
    /// </summary>
    public static string GetLocalizedName(string nameRu, string nameTj, string nameEn, Language language)
    {
        return language switch
        {
            Language. Russian => nameRu,
            Language.Tajik => nameTj,
            Language.English => nameEn,
            _ => nameRu
        };
    }

    /// <summary>
    /// Get localized description from entity
    /// </summary>
    public static string?  GetLocalizedDescription(
        string?  descriptionRu, 
        string? descriptionTj, 
        string? descriptionEn, 
        Language language)
    {
        return language switch
        {
            Language.Russian => descriptionRu,
            Language.Tajik => descriptionTj,
            Language.English => descriptionEn,
            _ => descriptionRu
        };
    }
}