namespace AccountsPOC.Domain.Entities;

public class PaymentProviderConfig
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string ProviderName { get; set; } // "Stripe", "ApplePay", "GooglePay", "PayPal", etc.
    public required string ProviderCode { get; set; } // Unique code: "STRIPE", "APPLE_PAY", "GOOGLE_PAY"
    public string? PublishableKey { get; set; }
    public string? SecretKey { get; set; }
    public string? ApiKey { get; set; }
    public string? MerchantId { get; set; }
    public string? WebhookSecret { get; set; }
    public string? Environment { get; set; } // "Test", "Production"
    public bool IsEnabled { get; set; }
    public bool IsDefault { get; set; }
    public string? AdditionalConfig { get; set; } // JSON for provider-specific settings
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation property
    public Tenant? Tenant { get; set; }
}
