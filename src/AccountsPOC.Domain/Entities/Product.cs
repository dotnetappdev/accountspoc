namespace AccountsPOC.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string ProductCode { get; set; }
    public required string ProductName { get; set; }
    public string? Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockLevel { get; set; }
    public int ReorderLevel { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public ICollection<BillOfMaterial> BillOfMaterials { get; set; } = new List<BillOfMaterial>();
}
