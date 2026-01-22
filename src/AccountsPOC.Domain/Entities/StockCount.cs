namespace AccountsPOC.Domain.Entities;

public class StockCount
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string CountNumber { get; set; }
    public DateTime CountDate { get; set; }
    public int? WarehouseId { get; set; }
    public string Status { get; set; } = "InProgress"; // InProgress, Completed, Reconciled
    public string? CountedByUserId { get; set; }
    public string? CountedByUserName { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    
    // Navigation properties
    public Warehouse? Warehouse { get; set; }
    public ICollection<StockCountItem> Items { get; set; } = new List<StockCountItem>();
}

public class StockCountItem
{
    public int Id { get; set; }
    public int StockCountId { get; set; }
    public int StockItemId { get; set; }
    public int ExpectedQuantity { get; set; }
    public int CountedQuantity { get; set; }
    public int VarianceQuantity => CountedQuantity - ExpectedQuantity;
    public string? Notes { get; set; }
    public DateTime? CountedDate { get; set; }
    
    // Navigation properties
    public StockCount? StockCount { get; set; }
    public StockItem? StockItem { get; set; }
}
