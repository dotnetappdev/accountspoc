namespace AccountsPOC.Domain.Entities;

public class PriceList
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string PriceListCode { get; set; }
    public required string Description { get; set; }
    public string? Currency { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public ICollection<PriceListItem> PriceListItems { get; set; } = new List<PriceListItem>();
}
