namespace SomonAI. Lib.Localization;

/// <summary>
/// Represents a localized string with Russian, Tajik and English variants
/// </summary>
public class LocalizedString
{
    /// <summary>
    /// Russian text
    /// </summary>
    [BsonElement("ru")]
    public string Russian { get; set; } = string.Empty;

    /// <summary>
    /// Tajik text
    /// </summary>
    [BsonElement("tj")]
    public string Tajik { get; set; } = string.Empty;

    /// <summary>
    /// English text
    /// </summary>
    [BsonElement("en")]
    public string English { get; set; } = string.Empty;

    /// <summary>
    /// Get text for specified language
    /// </summary>
    public string Get(Language language)
    {
        return language switch
        {
            Language.Russian => Russian,
            Language.Tajik => Tajik,
            Language.English => English,
            _ => Russian
        };
    }

    /// <summary>
    /// Get text for specified language code
    /// </summary>
    public string Get(string languageCode)
    {
        var language = LanguageExtensions.FromCode(languageCode);
        return Get(language);
    }

    /// <summary>
    /// Set text for specified language
    /// </summary>
    public void Set(Language language, string value)
    {
        switch (language)
        {
            case Language.Russian:
                Russian = value;
                break;
            case Language.Tajik:
                Tajik = value;
                break;
            case Language.English:
                English = value;
                break;
        }
    }

    /// <summary>
    /// Create localized string from Russian, Tajik and English texts
    /// </summary>
    public static LocalizedString Create(string russian, string tajik, string english)
    {
        return new LocalizedString
        {
            Russian = russian,
            Tajik = tajik,
            English = english
        };
    }

    /// <summary>
    /// Implicit conversion to string (returns Russian by default)
    /// </summary>
    public static implicit operator string(LocalizedString localizedString)
        => localizedString.Russian;

    public override string ToString() => Russian;
}