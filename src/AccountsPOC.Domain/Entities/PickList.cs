namespace AccountsPOC.Domain.Entities;

public class PickList
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string PickListNumber { get; set; }
    public int? SalesOrderId { get; set; }
    public DateTime PickListDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled
    public string? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public DateTime? StartedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public SalesOrder? SalesOrder { get; set; }
    public ICollection<PickListItem> Items { get; set; } = new List<PickListItem>();
}

public class PickListItem
{
    public int Id { get; set; }
    public int PickListId { get; set; }
    public int StockItemId { get; set; }
    public int QuantityRequired { get; set; }
    public int QuantityPicked { get; set; }
    public string? BinLocation { get; set; }
    public DateTime? PickedDate { get; set; }
    public string? PickedByUserId { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public PickList? PickList { get; set; }
    public StockItem? StockItem { get; set; }
}
