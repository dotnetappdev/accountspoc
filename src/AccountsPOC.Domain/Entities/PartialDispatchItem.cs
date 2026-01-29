namespace AccountsPOC.Domain.Entities;

public class PartialDispatchItem
{
    public int Id { get; set; }
    public int PartialDispatchId { get; set; }
    public int SalesOrderItemId { get; set; }
    public int QuantityDispatched { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public PartialDispatch? PartialDispatch { get; set; }
    public SalesOrderItem? SalesOrderItem { get; set; }
}
