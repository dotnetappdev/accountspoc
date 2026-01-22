namespace AccountsPOC.BlazorApp.Models;

public class PaymentProviderConfig
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string ProviderName { get; set; }
    public required string ProviderCode { get; set; }
    public string? PublishableKey { get; set; }
    public string? SecretKey { get; set; }
    public string? ApiKey { get; set; }
    public string? MerchantId { get; set; }
    public string? WebhookSecret { get; set; }
    public string? Environment { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsDefault { get; set; }
    public string? AdditionalConfig { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
