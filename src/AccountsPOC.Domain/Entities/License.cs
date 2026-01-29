namespace AccountsPOC.Domain.Entities;

public class License
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string LicenseKey { get; set; }
    public required string LicenseType { get; set; } // Starter, Professional, Enterprise, Custom
    public bool IsActive { get; set; } = true;
    public DateTime ActivationDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    
    // Installation Limits
    public int MaxInstallations { get; set; } = 1;
    public int CurrentInstallations { get; set; } = 0;
    
    // Stock Item Limits
    public int? MaxStockItems { get; set; } // null = unlimited
    public bool AllowMultipleImages { get; set; } = false;
    public int? MaxImagesPerStockItem { get; set; } // null = unlimited
    
    // User Limits
    public int? MaxUsers { get; set; } // null = unlimited
    public int? MaxRoles { get; set; } // null = unlimited
    
    // Customer & Tenant Limits
    public int? MaxCustomers { get; set; } // null = unlimited
    public int? MaxTenants { get; set; } // null = unlimited
    
    // Order Limits
    public int? MaxSalesOrdersPerMonth { get; set; } // null = unlimited
    public int? MaxPurchaseOrdersPerMonth { get; set; } // null = unlimited
    
    // Warehouse & Inventory Limits
    public int? MaxWarehouses { get; set; } // null = unlimited
    public int? MaxProducts { get; set; } // null = unlimited
    
    // Feature Flags
    public bool EnablePdfExport { get; set; } = true;
    public bool EnableEmailTemplates { get; set; } = true;
    public bool EnableCustomForms { get; set; } = false;
    public bool EnablePaymentIntegration { get; set; } = false;
    public bool EnableAdvancedReporting { get; set; } = false;
    public bool EnableApiAccess { get; set; } = false;
    public bool EnableMultipleCurrencies { get; set; } = false;
    
    // Custom Limits (JSON format for extensibility)
    public string? CustomLimits { get; set; }
    
    // Audit
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? Notes { get; set; }
    
    // Navigation property
    public Tenant Tenant { get; set; } = null!;
}
