using Azure.Identity;
using GameHubApi.Contracts;
using GameHubApi.Providers;
using GameHubApi.Services;
var builder = WebApplication.CreateBuilder(args);

var keyVaultVariable = Environment.GetEnvironmentVariable("SERVICE_KEYVAULT");
if (string.IsNullOrWhiteSpace(keyVaultVariable))
{
    throw new InvalidOperationException("Key Vault URL is not set in the environment variables.");
}

// If you need to access the secret in a service or controller, inject IConfiguration into the class and retrieve the value
var keyVaultEndpoint = new Uri(keyVaultVariable);
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

// Add services to the container.
builder.Services.AddCors(options =>
{
    // cors policy to only allow GET requests from allow-list origins
    options.AddPolicy("SpecificOrigins", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174",
                "https://gamers-hub.azurewebsites.net",
                "https://gamers-hub-cnt.azurewebsites.net") // allowed origins
            .WithMethods("GET") // Only allow GET 
            .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IGamesService, GamesService>();
builder.Services.AddScoped<IGenresService, GenresService>();
builder.Services.AddScoped<ITranslator, AzureTranslatorApi>();
builder.Services.AddScoped<ILargeLanguageModel, OpenAILargeLanguageModel>();

// Game filter is stateless and thread-safe, so we can register it as a singleton
builder.Services.AddSingleton<IGameFilter, GameFilter>();

// Register HttpClient for use by Providers
builder.Services.AddHttpClient();

// Register HttpContextAccessor to enable access to the current HTTP context
builder.Services.AddHttpContextAccessor();

// Register games cache as a Singleton so it can be reused across requests
builder.Services.AddSingleton<ILruCache<string, CollectionResult<Game>>>(provider =>
{
    return new LruCache<string, CollectionResult<Game>>(size: 500);
});
builder.Services.AddSingleton<ILruCache<string, CollectionResult<Genre>>>(provider =>
{
    return new LruCache<string, CollectionResult<Genre>>(size: 100);
});
builder.Services.AddSingleton<ILruCache<string, Game>>(provider =>
{
    return new LruCache<string, Game>(size: 100);
});
builder.Services.AddSingleton<ILruCache<string, CollectionResult<Movie>>>(provider =>
{
    return new LruCache<string, CollectionResult<Movie>>(size: 100);
});
builder.Services.AddSingleton<ILruCache<string, CollectionResult<Screenshot>>>(provider =>
{
    return new LruCache<string, CollectionResult<Screenshot>>(size: 100);
});

// Register CachedRawgApi as default implementation of the interface
builder.Services.AddScoped<IRawgApi, CachedRawgApi>();

// Register IRawgApi factory to enable creation of instances of RawgApi and CachedRawgApi based on the key provided.
// This allows for easy switching between different implementations of IRawgApi.
// For this to work, we must also register the concrete implementations so they can be resolved by the factory.
builder.Services.AddScoped<RawgApi>();
builder.Services.AddScoped<CachedRawgApi>();
builder.Services.AddScoped<Func<string, IRawgApi>>(serviceProvider => key =>
{
    return key switch
    {
        "Base" => serviceProvider.GetService<RawgApi>() ?? throw new InvalidOperationException("Service of type RawgApi is not registered."),
        "Cached" => serviceProvider.GetService<CachedRawgApi>() ?? throw new InvalidOperationException("Service of type CachedRawgApi is not registered."),
        _ => throw new ArgumentException("Invalid IRawgApi type")
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("SpecificOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
