namespace AccountsPOC.Domain.Entities;

public class PartialDispatch
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int SalesOrderId { get; set; }
    public required string DispatchNumber { get; set; }
    public DateTime DispatchDate { get; set; }
    
    // Carrier Information
    public string? CarrierName { get; set; }
    public string? TrackingNumber { get; set; }
    public string? ShippingMethod { get; set; }
    
    // Dispatch Details
    public decimal Weight { get; set; }
    public string? WeightUnit { get; set; }
    public int NumberOfPackages { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Dispatched, InTransit, Delivered
    
    // Address Information
    public string? DeliveryAddress { get; set; }
    public string? DeliveryCity { get; set; }
    public string? DeliveryPostCode { get; set; }
    public string? DeliveryCountry { get; set; }
    public string? DeliveryContactName { get; set; }
    public string? DeliveryContactPhone { get; set; }
    
    public string? Notes { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
    
    // Navigation properties
    public SalesOrder? SalesOrder { get; set; }
    public ICollection<PartialDispatchItem> PartialDispatchItems { get; set; } = new List<PartialDispatchItem>();
}
