namespace AccountsPOC.Domain.Entities;

public class ConfigurationSetting
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string Category { get; set; } // "System", "Payment", "Email", "ApiKey", "Tax", "Currency", etc.
    public required string Key { get; set; } // Unique within category
    public required string Value { get; set; } // Store as string, can be JSON for complex values
    public string? DataType { get; set; } // "String", "Number", "Boolean", "JSON", "Encrypted"
    public string? Description { get; set; }
    public bool IsEncrypted { get; set; } = false;
    public bool IsSystem { get; set; } = false; // System settings cannot be deleted
    public int DisplayOrder { get; set; } = 0;
    public string? ValidationRule { get; set; } // Optional validation (regex, range, etc.)
    public string? DefaultValue { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? LastModifiedBy { get; set; }
    
    // Navigation property
    public Tenant? Tenant { get; set; }
}
