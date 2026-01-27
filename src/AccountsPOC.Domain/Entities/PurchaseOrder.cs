namespace AccountsPOC.Domain.Entities;

public class PurchaseOrder
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    
    // Order Details
    public string Status { get; set; } = "Draft"; // Draft, Sent, Confirmed, PartiallyReceived, Received, Cancelled, OnHold
    public string? Priority { get; set; } // Low, Normal, High, Urgent
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TotalAmount { get; set; }
    
    // Sage 200 specific fields
    public string? CurrencyCode { get; set; }
    public decimal ExchangeRate { get; set; } = 1.0m;
    public string? SupplierReference { get; set; }
    public string? BuyerName { get; set; }
    public int? WarehouseId { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? DeliveryCity { get; set; }
    public string? DeliveryPostCode { get; set; }
    public string? DeliveryCountry { get; set; }
    public string? DeliveryContactName { get; set; }
    public string? DeliveryContactPhone { get; set; }
    public string? DeliveryInstructions { get; set; }
    
    // Payment Terms
    public string PaymentTerms { get; set; } = "Net 30";
    public string? PaymentMethod { get; set; }
    public bool PaymentCompleted { get; set; }
    public DateTime? PaymentDate { get; set; }
    
    // Order Type
    public string? OrderType { get; set; } // Standard, DropShip, DirectDelivery
    public string? ShippingMethod { get; set; }
    public string? CarrierName { get; set; }
    public string? TrackingNumber { get; set; }
    
    public string? Notes { get; set; }
    public string? InternalNotes { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public DateTime? SentDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
    
    // Navigation
    public Tenant? Tenant { get; set; }
    public Supplier? Supplier { get; set; }
    public Warehouse? Warehouse { get; set; }
    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
}
