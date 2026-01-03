namespace SomonAI. Lib.Localization;

/// <summary>
/// Language provider implementation using HTTP context
/// </summary>
public class LanguageProvider(IHttpContextAccessor httpContextAccessor) : ILanguageProvider
{
    private const string LanguageHeaderKey = "Accept-Language";
    private const string LanguageItemKey = "CurrentLanguage";

    /// <summary>
    /// Get current language from: 
    /// 1. HTTP Items (if set manually)
    /// 2. Accept-Language header
    /// 3. Default to Russian
    /// </summary>
    public Language GetCurrentLanguage()
    {
        HttpContext httpContext = httpContextAccessor.HttpContext;
        
        if (httpContext == null)
            return Language.Russian;

        if (httpContext.Items.TryGetValue(LanguageItemKey, out var languageItem) 
            && languageItem is Language setLanguage)
        {
            return setLanguage;
        }

        if (httpContext.Request.Headers.TryGetValue(LanguageHeaderKey, out var headerValue))
        {
            string? languageCode = headerValue.ToString().Split(',').FirstOrDefault()?.Trim();
            if (!string.IsNullOrEmpty(languageCode))
            {
                var language = LanguageExtensions. FromCode(languageCode);
                httpContext.Items[LanguageItemKey] = language;
                return language;
            }
        }

        if (httpContext.Request.Query.TryGetValue("lang", out var langQuery))
        {
            var language = LanguageExtensions.FromCode(langQuery. ToString());
            httpContext.Items[LanguageItemKey] = language;
            return language;
        }

        return Language.Russian;
    }

    /// <summary>
    /// Get current language code
    /// </summary>
    public string GetCurrentLanguageCode()
        => GetCurrentLanguage().GetCode();

    /// <summary>
    /// Set language for current request
    /// </summary>
    public void SetLanguage(Language language)
    {
        HttpContext httpContext = httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            httpContext.Items[LanguageItemKey] = language;
        }
    }

    /// <summary>
    /// Set language from code
    /// </summary>
    public void SetLanguage(string languageCode)
    {
        Language language = LanguageExtensions.FromCode(languageCode);
        SetLanguage(language);
    }
}