namespace AccountsPOC.Domain.Entities;

public class SalesOrder
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public DateTime? PromisedDate { get; set; }
    
    // Customer Information
    public int? CustomerId { get; set; }
    public required string CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerReference { get; set; }
    
    // Delivery Information
    public string? DeliveryAddress { get; set; }
    public string? DeliveryCity { get; set; }
    public string? DeliveryPostCode { get; set; }
    public string? DeliveryCountry { get; set; }
    public string? DeliveryContactName { get; set; }
    public string? DeliveryContactPhone { get; set; }
    public string? DeliveryInstructions { get; set; }
    
    // Order Details
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TotalAmount { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal ExchangeRate { get; set; } = 1.0m;
    
    // Status and Processing
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Processing, Dispatched, Completed, Cancelled, OnHold
    public string? Priority { get; set; } // Low, Normal, High, Urgent
    public int? SalesPersonId { get; set; }
    public int? WarehouseId { get; set; }
    
    // Payment Information
    public string PaymentTerms { get; set; } = "Net 30";
    public string? PaymentMethod { get; set; }
    public bool PaymentReceived { get; set; }
    public DateTime? PaymentReceivedDate { get; set; }
    
    // Sage 200 specific fields
    public string? OrderType { get; set; } // Standard, BackOrder, DropShip, etc.
    public string? ShippingMethod { get; set; }
    public string? CarrierName { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? ShippedDate { get; set; }
    public string? InternalNotes { get; set; }
    public string? CustomerNotes { get; set; }
    
    // Linked BOMs - Innovation beyond Sage 200
    public bool HasLinkedBOMs { get; set; }
    public string? BOMProcessingStatus { get; set; } // Pending, InProgress, Completed
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
    
    // Navigation properties
    public Customer? Customer { get; set; }
    public Warehouse? Warehouse { get; set; }
    public ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();
    public ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
}
