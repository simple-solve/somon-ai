namespace SomonAI.API.Infrastructure.Middlewares;

public class LanguageMiddleware(RequestDelegate next, ILogger<LanguageMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, ILanguageProvider languageProvider)
    {
        var language = DetectLanguage(context);
        
        languageProvider.SetLanguage(language);
        
        logger.LogDebug("Language detected: {Language} from header:  {Header}", 
            language. GetCode(), 
            context.Request.Headers["X-Language"].ToString());
        
        await next(context);
    }

    private Language DetectLanguage(HttpContext context)
    {
        if (context.Request.Query. TryGetValue("lang", out var queryLang))
        {
            return LanguageExtensions.FromCode(queryLang. ToString());
        }

        if (context. Request.Headers.TryGetValue("X-Language", out var customHeader))
        {
            return LanguageExtensions.FromCode(customHeader.ToString());
        }

        if (context. Request.Headers.TryGetValue("Accept-Language", out var acceptLang))
        {
            var langCode = acceptLang.ToString().Split(',')[0].Split(';')[0].Trim();
            if (! string.IsNullOrEmpty(langCode))
            {
                return LanguageExtensions.FromCode(langCode);
            }
        }

        return Language.Russian;
    }
}