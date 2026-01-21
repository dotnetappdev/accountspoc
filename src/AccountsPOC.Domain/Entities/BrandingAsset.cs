namespace AccountsPOC.Domain.Entities;

public class BrandingAsset
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    
    /// <summary>
    /// Type of branding asset (Logo, EmailHeader, EmailFooter, Favicon, InvoiceLogo, etc.)
    /// </summary>
    public required string AssetType { get; set; }
    
    /// <summary>
    /// Friendly name for the asset
    /// </summary>
    public required string AssetName { get; set; }
    
    /// <summary>
    /// File name of the asset
    /// </summary>
    public string? FileName { get; set; }
    
    /// <summary>
    /// MIME type (image/png, image/jpeg, image/svg+xml, etc.)
    /// </summary>
    public string? MimeType { get; set; }
    
    /// <summary>
    /// Base64 encoded image data or file path
    /// </summary>
    public string? ImageData { get; set; }
    
    /// <summary>
    /// Alternative text for accessibility
    /// </summary>
    public string? AltText { get; set; }
    
    /// <summary>
    /// Width in pixels (optional)
    /// </summary>
    public int? Width { get; set; }
    
    /// <summary>
    /// Height in pixels (optional)
    /// </summary>
    public int? Height { get; set; }
    
    /// <summary>
    /// Whether this is the active/primary asset of its type
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Display order for multiple assets of same type
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long? FileSizeBytes { get; set; }
    
    /// <summary>
    /// Description or notes about the asset
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Usage context (web, email, print, mobile)
    /// </summary>
    public string? UsageContext { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
    
    // Navigation property
    public Tenant? Tenant { get; set; }
}
