namespace AccountsPOC.Domain.Entities;

public class SalesOrderItem
{
    public int Id { get; set; }
    public int SalesOrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int? BillOfMaterialId { get; set; }
    
    // Navigation properties
    public SalesOrder? SalesOrder { get; set; }
    public Product? Product { get; set; }
    public BillOfMaterial? BillOfMaterial { get; set; }
}
