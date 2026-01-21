namespace AccountsPOC.BlazorApp.Models;

public class ApiKeyConfig
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string ServiceName { get; set; }
    public required string KeyName { get; set; }
    public required string KeyType { get; set; }
    public required string KeyValue { get; set; }
    public string? MaskedKeyValue { get; set; }
    public string? Description { get; set; }
    public string? Environment { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ExpiryDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public DateTime? LastUsedDate { get; set; }
}
