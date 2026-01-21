namespace AccountsPOC.BlazorApp.Models;

public class SystemSettings
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string DefaultCurrency { get; set; } = "USD";
    public string? CurrencySymbol { get; set; } = "$";
    public int CurrencyDecimalPlaces { get; set; } = 2;
    public string DateFormat { get; set; } = "MM/dd/yyyy";
    public string CompanyName { get; set; } = "";
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
}
