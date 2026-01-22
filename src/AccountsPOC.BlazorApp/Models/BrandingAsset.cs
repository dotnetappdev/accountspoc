namespace AccountsPOC.BlazorApp.Models;

public class BrandingAsset
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public string? ImageData { get; set; }
    public string? AltText { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public long? FileSizeBytes { get; set; }
    public string? Description { get; set; }
    public string? UsageContext { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
}

public class BrandingAssetUploadRequest
{
    public string AssetType { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public string ImageDataBase64 { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public string? Description { get; set; }
    public string? UsageContext { get; set; }
}
