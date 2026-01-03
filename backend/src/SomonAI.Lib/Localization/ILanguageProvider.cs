namespace SomonAI.Lib.Localization;

/// <summary>
/// Provides current language context for the request
/// </summary>
public interface ILanguageProvider
{
    /// <summary>
    /// Get current language from HTTP context
    /// </summary>
    Language GetCurrentLanguage();

    /// <summary>
    /// Get current language code (ru/tj)
    /// </summary>
    string GetCurrentLanguageCode();

    /// <summary>
    /// Set language for current request
    /// </summary>
    void SetLanguage(Language language);

    /// <summary>
    /// Set language from code
    /// </summary>
    void SetLanguage(string languageCode);
}