namespace AccountsPOC.BlazorApp.Models;

public class ConfigurationSetting
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string Category { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }
    public string? DataType { get; set; }
    public string? Description { get; set; }
    public bool IsEncrypted { get; set; } = false;
    public bool IsSystem { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;
    public string? ValidationRule { get; set; }
    public string? DefaultValue { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? LastModifiedBy { get; set; }
}
