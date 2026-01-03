namespace SomonAI.Lib.Localization;

/// <summary>
/// Supported languages in the application
/// </summary>
public enum Language
{
    /// <summary>
    /// Russian language
    /// </summary>
    Russian = 1,

    /// <summary>
    /// Tajik language
    /// </summary>
    Tajik = 2,
    English = 3,
}

/// <summary>
/// Language extensions and utilities
/// </summary>
public static class LanguageExtensions
{
    /// <summary>
    /// Get language code (ISO 639-1)
    /// </summary>
    public static string GetCode(this Language language)
    {
        return language switch
        {
            Language.Russian => "ru",
            Language.Tajik => "tj",
            Language.English => "eng",
            _ => "ru"
        };
    }

    /// <summary>
    /// Get language display name
    /// </summary>
    public static string GetDisplayName(this Language language)
    {
        return language switch
        {
            Language.Russian => "Русский",
            Language. Tajik => "Тоҷикӣ",
            Language.English => "English",
            _ => "Русский"
        };
    }

    /// <summary>
    /// Get language from code
    /// </summary>
    public static Language FromCode(string? code)
    {
        return code?. ToLower() switch
        {
            "ru" or "rus" or "russian" => Language.Russian,
            "tj" or "tg" or "tajik" or "тоҷикӣ" => Language.Tajik,
            "eng" or "en" or "english" => Language.English,
            _ => Language.Russian // Default
        };
    }

    /// <summary>
    /// Get all available languages
    /// </summary>
    public static IEnumerable<Language> GetAll()
        => [Language.Russian, Language. Tajik,Language.English];
}