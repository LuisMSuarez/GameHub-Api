using Azure.Identity;
using GameHubApi.Contracts;
using GameHubApi.Providers;
using GameHubApi.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenAI.Chat;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Retrieve Key Vault endpoint from environment variable
var keyVaultVariable = Environment.GetEnvironmentVariable("SERVICE_KEYVAULT");
if (string.IsNullOrWhiteSpace(keyVaultVariable))
{
    // Fail fast if Key Vault URL is missing
    throw new InvalidOperationException("Key Vault URL is not set in the environment variables.");
}

// If you need to access the secret in a service or controller, inject IConfiguration into the class and retrieve the value
var keyVaultEndpoint = new Uri(keyVaultVariable);
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

// Add services to the container.
builder.Services.AddCors(options =>
{
    // cors policy
    options.AddPolicy("SpecificOrigins", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174",
                "https://gamers-hub.azurewebsites.net",
                "https://gamers-hub-cnt.azurewebsites.net") // allowed origins
            .WithMethods("GET", "POST")
            .AllowAnyHeader();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Microsoft";
})
.AddCookie()
.AddOpenIdConnect("Microsoft", options =>
{
    options.ClientId = builder.Configuration["ClientId"];
    options.ClientSecret = builder.Configuration["ClientSecret"];

    // MSAL treats the Microsoft account system (Live, MSA) as another tenant within the Microsoft identity platform. The tenant id of the Microsoft account tenant is: 9188040d-6c67-4c5b-b112-36a304b66dad
    options.Authority = "https://login.microsoftonline.com/9188040d-6c67-4c5b-b112-36a304b66dad/v2.0";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.Scope.Add("email");
    options.Scope.Add("profile");
    options.CallbackPath = "/signin-oidc";
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Authority = "https://login.microsoftonline.com/9188040d-6c67-4c5b-b112-36a304b66dad/v2.0";
    options.Audience = "api://739fc281-af8d-42f7-ac40-9cb0601ec826";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

// Add MVC controller support
builder.Services.AddControllers();

// Register core service dependencies
builder.Services.AddScoped<IGamesService, GamesService>();
builder.Services.AddScoped<IGenresService, GenresService>();
builder.Services.AddScoped<ITranslator, AzureTranslatorApi>();
builder.Services.AddScoped<ILargeLanguageModel, OpenAILargeLanguageModel>();

// Register IGameFilter factory to enable creation of instances of GameFilter and AIGameFilter based on the key provided.
// This allows for easy switching between different implementations of IGameFilter.
// For this to work, we must also register the concrete implementations so they can be resolved by the factory.
// Game filter is stateless and thread-safe, so we can register it as a singleton
builder.Services.AddScoped<IGameFilter, GameFilter>();
builder.Services.AddScoped<AIGameFilter>();
builder.Services.AddScoped<Func<string, IGameFilter>>(serviceProvider => key =>
{
    return key switch
    {
        "Base" => serviceProvider.GetService<GameFilter>() ?? throw new InvalidOperationException("Service of type GameFilter is not registered."),
        "AI" => serviceProvider.GetService<AIGameFilter>() ?? throw new InvalidOperationException("Service of type AIGameFilter is not registered."),
        _ => throw new ArgumentException("Invalid IGameFilter type")
    };
});

// Register OpenAI ChatClient using API key from Key Vault
builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var apiKey = config["OpenAIKey"];
    return new ChatClient(model: "gpt-4o-mini", apiKey: apiKey);
});

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

// Apply CORS policy to incoming requests
app.UseCors("SpecificOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI in development for API exploration
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enforce HTTPS and authorization middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes
app.MapControllers();

// Start the application
app.Run();
