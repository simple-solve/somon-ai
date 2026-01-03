using SomonAI.API.Infrastructure.Middlewares;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(MongoDbSettings.SectionName));

builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddScoped<DbInitializer>();

builder.Services.AddLocalization();
builder.Services.AddLocalizationServices();
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Somon AI API",
        Version = "v1",
        Description = "Marketplace API with multilingual support (ru/tj/en)"
    });

    options. OperationFilter<LanguageHeaderFilter>();
    
    options.EnableAnnotations();
    
    options.CustomSchemaIds(GetSchemaId);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("X-Language");
    });
});

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Somon AI API v1");
    c.RoutePrefix = "swagger";
});

app.UseStaticFiles();

app.UseCors("AllowAll");
app.UseMiddleware<LanguageMiddleware>();
app.UseAuthorization();

app.MapControllers();

await app.Services.InitializeDatabaseAsync();

app.Run();

static string GetSchemaId(Type type)
{
    if (type. IsGenericType)
    {
        var genericTypeName = type.Name.Split('`')[0];
        var genericArgs = type.GetGenericArguments();
        
        var argNames = string.Join("", genericArgs.Select(GetSchemaId));
        
        return $"{genericTypeName}Of{argNames}";
    }

    return type.Name
        .Replace("Dto", "")
        .Replace("Result", "")
        .Replace("Response", "");
}