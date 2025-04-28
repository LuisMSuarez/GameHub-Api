using Azure.Identity;
using GameHubApi.Services;
var builder = WebApplication.CreateBuilder(args);

var keyVaultVariable = Environment.GetEnvironmentVariable("SERVICE_KEYVAULT");
if (string.IsNullOrEmpty(keyVaultVariable))
{
    throw new InvalidOperationException("Key Vault URL is not set in the environment variables.");
}

// If you need to access the secret in a service or controller, inject IConfiguration into the class and retrieve the value
var keyVaultEndpoint = new Uri(keyVaultVariable);
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

// Add services to the container.
builder.Services.AddCors(options =>
{
    /* cors policy to allow all origins
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    */

    // cors policy to only allow GET requests from allow-list origins
    options.AddPolicy("SpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://gamers-hub.azurewebsites.net") // allowed origins
                .WithMethods("GET") // Only allow GET 
                .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IGamesService, GamesService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// app.UseCors("AllowAll");
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
