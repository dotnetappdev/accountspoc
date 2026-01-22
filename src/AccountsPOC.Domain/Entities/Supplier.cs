namespace AccountsPOC.Domain.Entities;

public class Supplier
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string SupplierCode { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    
    // Web integration
    public string? WebsiteUrl { get; set; }
    public string? ApiEndpoint { get; set; }
    public string? ApiUsername { get; set; }
    public string? ApiPassword { get; set; }
    
    // Reordering settings
    public int LeadTimeDays { get; set; }
    public decimal MinimumOrderValue { get; set; }
    public string? PaymentTerms { get; set; }
    
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation
    public Tenant? Tenant { get; set; }
}
