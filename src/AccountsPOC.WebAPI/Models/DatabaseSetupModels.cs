namespace AccountsPOC.WebAPI.Models;

public class DatabaseSetupRequest
{
    public string Server { get; set; } = "localhost";
    public string? Port { get; set; }
    public string Database { get; set; } = "AccountsPOCDb";
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool TrustedConnection { get; set; } = true;
    public bool TrustServerCertificate { get; set; } = true;
    public bool Encrypt { get; set; } = false;
    public bool IntegratedSecurity { get; set; } = true;
    public string DatabaseProvider { get; set; } = "SqlServer"; // SqlServer, SQLite, PostgreSQL
    public bool SeedUserData { get; set; } = false;
}

public class DatabaseSetupResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ConnectionString { get; set; }
    public int? UsersSeeded { get; set; }
    public int? RolesSeeded { get; set; }
}

public class ConnectionTestRequest
{
    public required string ConnectionString { get; set; }
    public string DatabaseProvider { get; set; } = "SqlServer";
}

public class ConnectionTestResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ServerVersion { get; set; }
}
