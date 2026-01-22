namespace AccountsPOC.Domain.Entities;

public class ApiKeyConfig
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string ServiceName { get; set; } // "Stripe", "SendGrid", "Twilio", "AWS", etc.
    public required string KeyName { get; set; } // "Publishable Key", "Secret Key", "API Key"
    public required string KeyType { get; set; } // "PublishableKey", "SecretKey", "ApiKey", "AccessToken"
    public required string KeyValue { get; set; } // The actual key (should be encrypted in production)
    public string? Description { get; set; }
    public string? Environment { get; set; } // "Development", "Staging", "Production"
    public bool IsActive { get; set; } = true;
    public DateTime? ExpiryDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public DateTime? LastUsedDate { get; set; }
    
    // Navigation property
    public Tenant? Tenant { get; set; }
}
