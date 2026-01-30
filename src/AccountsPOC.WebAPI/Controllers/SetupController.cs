using AccountsPOC.Infrastructure.Data;
using AccountsPOC.WebAPI.Models;
using AccountsPOC.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SetupController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public SetupController(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    [HttpPost("build-connection-string")]
    public ActionResult<string> BuildConnectionString([FromBody] DatabaseSetupRequest request)
    {
        try
        {
            string connectionString = request.DatabaseProvider.ToLower() switch
            {
                "sqlserver" => BuildSqlServerConnectionString(request),
                "sqlite" => BuildSqliteConnectionString(request),
                "postgresql" => BuildPostgreSqlConnectionString(request),
                _ => throw new ArgumentException("Unsupported database provider")
            };

            return Ok(new { connectionString });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("test-connection")]
    public async Task<ActionResult<ConnectionTestResponse>> TestConnection([FromBody] ConnectionTestRequest request)
    {
        try
        {
            if (request.DatabaseProvider.ToLower() == "sqlserver")
            {
                using var connection = new SqlConnection(request.ConnectionString);
                await connection.OpenAsync();
                
                var version = connection.ServerVersion;
                await connection.CloseAsync();

                return Ok(new ConnectionTestResponse
                {
                    Success = true,
                    Message = "Connection successful",
                    ServerVersion = version
                });
            }
            else if (request.DatabaseProvider.ToLower() == "sqlite")
            {
                // For SQLite, just check if we can create a context
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlite(request.ConnectionString);
                
                using var context = new ApplicationDbContext(optionsBuilder.Options);
                await context.Database.CanConnectAsync();

                return Ok(new ConnectionTestResponse
                {
                    Success = true,
                    Message = "Connection successful",
                    ServerVersion = "SQLite"
                });
            }
            else
            {
                return BadRequest(new ConnectionTestResponse
                {
                    Success = false,
                    Message = "Unsupported database provider"
                });
            }
        }
        catch (Exception ex)
        {
            return Ok(new ConnectionTestResponse
            {
                Success = false,
                Message = $"Connection failed: {ex.Message}"
            });
        }
    }

    [HttpPost("apply-configuration")]
    public async Task<ActionResult<DatabaseSetupResponse>> ApplyConfiguration([FromBody] DatabaseSetupRequest request)
    {
        try
        {
            // Build connection string
            string connectionString = request.DatabaseProvider.ToLower() switch
            {
                "sqlserver" => BuildSqlServerConnectionString(request),
                "sqlite" => BuildSqliteConnectionString(request),
                "postgresql" => BuildPostgreSqlConnectionString(request),
                _ => throw new ArgumentException("Unsupported database provider")
            };

            // Test connection
            var testRequest = new ConnectionTestRequest
            {
                ConnectionString = connectionString,
                DatabaseProvider = request.DatabaseProvider
            };
            
            var testResult = await TestConnection(testRequest);
            if (testResult.Value?.Success != true)
            {
                return BadRequest(new DatabaseSetupResponse
                {
                    Success = false,
                    Message = $"Connection test failed: {testResult.Value?.Message}"
                });
            }

            // Create database context with new connection string
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            
            if (request.DatabaseProvider.ToLower() == "sqlserver")
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
            else if (request.DatabaseProvider.ToLower() == "sqlite")
            {
                optionsBuilder.UseSqlite(connectionString);
            }

            using var context = new ApplicationDbContext(optionsBuilder.Options);
            
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            var response = new DatabaseSetupResponse
            {
                Success = true,
                Message = "Database configured successfully",
                ConnectionString = connectionString
            };

            // Save configuration to file for runtime use
            var configFilePath = _configuration["DatabaseSetup:ConfigFilePath"] ?? "database-config.json";
            var fullConfigPath = Path.Combine(Directory.GetCurrentDirectory(), configFilePath);
            
            var dbConfig = new DatabaseConfiguration
            {
                ConnectionString = connectionString,
                DatabaseProvider = request.DatabaseProvider,
                ConfiguredDate = DateTime.UtcNow,
                ConfiguredBy = "Setup"
            };
            
            var configJson = System.Text.Json.JsonSerializer.Serialize(dbConfig, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
            await System.IO.File.WriteAllTextAsync(fullConfigPath, configJson);

            // Seed basic data if requested
            if (request.SeedBasicData || request.SeedUserData)
            {
                using var scope = _serviceProvider.CreateScope();
                var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                
                // Seed basic setup (tenants, warehouses, etc.) if requested
                if (request.SeedBasicData)
                {
                    await seeder.SeedBasicDataAsync();
                }
                
                // Seed full user data if requested
                if (request.SeedUserData)
                {
                    await seeder.SeedAsync();
                }

                // Count seeded data
                var userCount = await context.Users.CountAsync();
                var roleCount = await context.Roles.CountAsync();

                response.UsersSeeded = userCount;
                response.RolesSeeded = roleCount;
                
                if (request.SeedBasicData && request.SeedUserData)
                {
                    response.Message += $" | Seeded {userCount} users, {roleCount} roles and basic data";
                }
                else if (request.SeedUserData)
                {
                    response.Message += $" | Seeded {userCount} users and {roleCount} roles";
                }
                else if (request.SeedBasicData)
                {
                    response.Message += " | Seeded basic data (tenants, warehouses, etc.)";
                }
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new DatabaseSetupResponse
            {
                Success = false,
                Message = $"Configuration failed: {ex.Message}"
            });
        }
    }

    private string BuildSqlServerConnectionString(DatabaseSetupRequest request)
    {
        var builder = new StringBuilder();
        
        builder.Append($"Server={request.Server}");
        
        if (!string.IsNullOrEmpty(request.Port))
        {
            builder.Append($",{request.Port}");
        }
        
        builder.Append($";Database={request.Database}");

        if (request.TrustedConnection || request.IntegratedSecurity)
        {
            builder.Append(";Integrated Security=true");
        }
        else if (!string.IsNullOrEmpty(request.Username) && !string.IsNullOrEmpty(request.Password))
        {
            builder.Append($";User Id={request.Username};Password={request.Password}");
        }

        if (request.TrustServerCertificate)
        {
            builder.Append(";TrustServerCertificate=true");
        }

        if (request.Encrypt)
        {
            builder.Append(";Encrypt=true");
        }
        else
        {
            builder.Append(";Encrypt=false");
        }

        return builder.ToString();
    }

    private string BuildSqliteConnectionString(DatabaseSetupRequest request)
    {
        return $"Data Source={request.Database}.db";
    }

    private string BuildPostgreSqlConnectionString(DatabaseSetupRequest request)
    {
        var builder = new StringBuilder();
        
        builder.Append($"Host={request.Server}");
        
        if (!string.IsNullOrEmpty(request.Port))
        {
            builder.Append($";Port={request.Port}");
        }
        else
        {
            builder.Append(";Port=5432");
        }
        
        builder.Append($";Database={request.Database}");

        if (!string.IsNullOrEmpty(request.Username) && !string.IsNullOrEmpty(request.Password))
        {
            builder.Append($";Username={request.Username};Password={request.Password}");
        }

        if (request.TrustServerCertificate)
        {
            builder.Append(";Trust Server Certificate=true");
        }

        if (request.Encrypt)
        {
            builder.Append(";SSL Mode=Require");
        }

        return builder.ToString();
    }
}
