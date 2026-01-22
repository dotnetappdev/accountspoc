namespace AccountsPOC.BlazorApp.Models;

public class StockItem
{
    public int Id { get; set; }
    public required string StockCode { get; set; }
    public required string Description { get; set; }
    public string? LongDescription { get; set; }
    public int ProductId { get; set; }
    public int? WarehouseId { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int QuantityOnHand { get; set; }
    public int QuantityAllocated { get; set; }
    public int QuantityAvailable => QuantityOnHand - QuantityAllocated;
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public string? BinLocation { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public Product? Product { get; set; }
    public Warehouse? Warehouse { get; set; }
}
