namespace AccountsPOC.Domain.Entities;

public class PurchaseOrder
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    
    public string Status { get; set; } = "Draft"; // Draft, Sent, Confirmed, PartiallyReceived, Received, Cancelled
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    
    public string? Notes { get; set; }
    public string? DeliveryAddress { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public DateTime? SentDate { get; set; }
    
    // Navigation
    public Tenant? Tenant { get; set; }
    public Supplier? Supplier { get; set; }
    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
}
