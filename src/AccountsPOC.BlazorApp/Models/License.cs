namespace AccountsPOC.BlazorApp.Models;

public class License
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string LicenseKey { get; set; } = "";
    public string LicenseType { get; set; } = "";
    public bool IsActive { get; set; }
    public DateTime ActivationDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int MaxInstallations { get; set; }
    public int CurrentInstallations { get; set; }
    public int? MaxStockItems { get; set; }
    public bool AllowMultipleImages { get; set; }
    public int? MaxImagesPerStockItem { get; set; }
    public int? MaxUsers { get; set; }
    public int? MaxRoles { get; set; }
    public int? MaxCustomers { get; set; }
    public int? MaxTenants { get; set; }
    public int? MaxSalesOrdersPerMonth { get; set; }
    public int? MaxPurchaseOrdersPerMonth { get; set; }
    public int? MaxWarehouses { get; set; }
    public int? MaxProducts { get; set; }
    public bool EnablePdfExport { get; set; }
    public bool EnableEmailTemplates { get; set; }
    public bool EnableCustomForms { get; set; }
    public bool EnablePaymentIntegration { get; set; }
    public bool EnableAdvancedReporting { get; set; }
    public bool EnableApiAccess { get; set; }
    public bool EnableMultipleCurrencies { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}

public class Installation
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int LicenseId { get; set; }
    public string InstallationKey { get; set; } = "";
    public string MachineName { get; set; } = "";
    public string? MachineIdentifier { get; set; }
    public string? IpAddress { get; set; }
    public string? Version { get; set; }
    public bool IsActive { get; set; }
    public DateTime ActivationDate { get; set; }
    public DateTime? DeactivationDate { get; set; }
    public DateTime? LastHeartbeat { get; set; }
    public DateTime CreatedDate { get; set; }
}
