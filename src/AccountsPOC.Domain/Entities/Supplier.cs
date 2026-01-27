namespace AccountsPOC.Domain.Entities;

public class Supplier
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string SupplierCode { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string? ContactTitle { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    public string? Fax { get; set; }
    
    // Address Information
    public string Address { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? County { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    
    // Tax and Financial
    public string? VATNumber { get; set; }
    public string? TaxCode { get; set; }
    public string? AccountNumber { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankSortCode { get; set; }
    
    // Sage 200 Supplier Fields
    public string? CurrencyCode { get; set; }
    public string? SupplierGroup { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public bool OnHold { get; set; }
    public string? OnHoldReason { get; set; }
    
    // Web integration
    public string? WebsiteUrl { get; set; }
    public string? ApiEndpoint { get; set; }
    public string? ApiUsername { get; set; }
    public string? ApiPassword { get; set; }
    
    // Reordering settings
    public int LeadTimeDays { get; set; }
    public decimal MinimumOrderValue { get; set; }
    public string? PaymentTerms { get; set; }
    public int PaymentTermsDays { get; set; } = 30;
    public string? DeliveryTerms { get; set; }
    public string? PreferredDeliveryMethod { get; set; }
    
    // Performance Tracking
    public decimal? AverageDeliveryDays { get; set; }
    public decimal? QualityRating { get; set; }
    public DateTime? LastOrderDate { get; set; }
    public string? Notes { get; set; }
    
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation
    public Tenant? Tenant { get; set; }
}
