namespace AccountsPOC.BlazorApp.Models;

public class Warehouse
{
    public int Id { get; set; }
    public required string WarehouseCode { get; set; }
    public required string WarehouseName { get; set; }
    public required string Address { get; set; }
    public string? City { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
}
