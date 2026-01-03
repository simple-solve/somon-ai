namespace SomonAI.API.Infrastructure. Swagger;

/// <summary>
/// Adds X-Language header to all Swagger operations
/// </summary>
public sealed class LanguageHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation. Parameters.Add(new OpenApiParameter
        {
            Name = "X-Language",
            In = ParameterLocation.Header,
            Description = "Язык ответа / Response language\n\n" +
                          "- `ru` - Русский (default)\n" +
                          "- `tj` - Тоҷикӣ\n" +
                          "- `en` - English",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Enum = new List<IOpenApiAny>
                {
                    new OpenApiString("ru"),
                    new OpenApiString("tj"),
                    new OpenApiString("en")
                },
                Default = new OpenApiString("ru")
            }
        });
    }
}