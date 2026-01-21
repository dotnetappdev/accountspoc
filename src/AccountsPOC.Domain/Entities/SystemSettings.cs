namespace AccountsPOC.Domain.Entities;

public class SystemSettings
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string DefaultCurrency { get; set; }
    public string? CurrencySymbol { get; set; }
    public int CurrencyDecimalPlaces { get; set; } = 2;
    public required string DateFormat { get; set; }
    public required string CompanyName { get; set; }
    public string? CompanyLogo { get; set; }
    public string? CompanyAddress { get; set; }
    public string? TaxNumber { get; set; }
    public decimal DefaultTaxRate { get; set; }
    public string? EmailFromAddress { get; set; }
    public string? EmailFromName { get; set; }
    public bool EnablePaymentIntegration { get; set; }
    public string? StripePublishableKey { get; set; }
    public string? StripeSecretKey { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation property
    public Tenant? Tenant { get; set; }
}
