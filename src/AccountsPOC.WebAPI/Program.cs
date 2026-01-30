using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using AccountsPOC.PdfGenerator.Services;
using AccountsPOC.WebAPI.Models;
using AccountsPOC.WebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add Sentry for error tracking and diagnostics
var sentryDsn = builder.Configuration["Sentry:Dsn"];
if (!string.IsNullOrEmpty(sentryDsn))
{
    builder.WebHost.UseSentry(options =>
    {
        options.Dsn = sentryDsn;
        options.Environment = builder.Configuration["Sentry:Environment"] ?? "Development";
        options.TracesSampleRate = double.Parse(builder.Configuration["Sentry:TracesSampleRate"] ?? "1.0");
        options.SendDefaultPii = bool.Parse(builder.Configuration["Sentry:SendDefaultPii"] ?? "false");
        options.MaxBreadcrumbs = int.Parse(builder.Configuration["Sentry:MaxBreadcrumbs"] ?? "100");
        options.Debug = bool.Parse(builder.Configuration["Sentry:Debug"] ?? "false");
        options.AttachStacktrace = bool.Parse(builder.Configuration["Sentry:AttachStacktrace"] ?? "true");
    });
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add HttpClient for external API calls
builder.Services.AddHttpClient();

// Configure database connection
string connectionString;
string databaseProvider = "SQLite"; // Default for local development

// Check if runtime configuration is enabled
var allowRuntimeConfig = builder.Configuration.GetValue<bool>("DatabaseSetup:AllowRuntimeConfiguration", true);
var configFilePath = builder.Configuration["DatabaseSetup:ConfigFilePath"] ?? "database-config.json";
var fullConfigPath = Path.Combine(Directory.GetCurrentDirectory(), configFilePath);

if (allowRuntimeConfig && File.Exists(fullConfigPath))
{
    try
    {
        // Read from configuration file saved by setup
        var configJson = await File.ReadAllTextAsync(fullConfigPath);
        var dbConfig = JsonSerializer.Deserialize<DatabaseConfiguration>(configJson);
        
        if (dbConfig != null && !string.IsNullOrEmpty(dbConfig.ConnectionString))
        {
            connectionString = dbConfig.ConnectionString;
            databaseProvider = dbConfig.DatabaseProvider;
        }
        else
        {
            // Fallback to appsettings
            connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=AccountsPOC.db";
        }
    }
    catch (Exception ex)
    {
        // Log error and fallback to appsettings
        Console.WriteLine($"Error reading database configuration: {ex.Message}");
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=AccountsPOC.db";
    }
}
else
{
    // Use connection string from appsettings
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=AccountsPOC.db";
}

// Add DbContext with appropriate provider
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(connectionString);
    }
    else if (databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        // Default to SQLite for local development
        options.UseSqlite(connectionString);
    }
});

// Add ASP.NET Identity
builder.Services.AddIdentity<User, Role>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Sign-in settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "DefaultSecretKeyForDevelopmentPleaseChangeInProduction123456";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "AccountsPOC";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "AccountsPOCApp";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// Add data seeder
builder.Services.AddScoped<DataSeeder>();

// Add PDF generator service
builder.Services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();

// Add License service
builder.Services.AddScoped<ILicenseService, LicenseService>();

// Add JWT token service
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("https://localhost:5001", "http://localhost:5000", "http://localhost:5193")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created and seeded only in development
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    try
    {
        dbContext.Database.EnsureCreated();
        
        // Seed data in development only if not already seeded
        if (app.Environment.IsDevelopment())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            await seeder.SeedAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating/seeding the database");
    }
}

app.Run();
