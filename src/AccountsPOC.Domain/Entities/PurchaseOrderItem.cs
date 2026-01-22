namespace AccountsPOC.Domain.Entities;

public class PurchaseOrderItem
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal LineTotal { get; set; }
    
    public int? QuantityReceived { get; set; }
    public DateTime? ReceivedDate { get; set; }
    
    // Navigation
    public PurchaseOrder? PurchaseOrder { get; set; }
    public Product? Product { get; set; }
}
