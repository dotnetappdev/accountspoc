namespace AccountsPOC.Domain.Entities;

public class DeliveryRoute
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string RouteNumber { get; set; }
    public DateTime RouteDate { get; set; }
    public string Status { get; set; } = "Planned"; // Planned, InProgress, Completed
    public int? DriverId { get; set; }
    public string? VehicleRegistration { get; set; }
    public DateTime? StartedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public Driver? Driver { get; set; }
    public ICollection<DeliveryStop> Stops { get; set; } = new List<DeliveryStop>();
}

public class DeliveryStop
{
    public int Id { get; set; }
    public int DeliveryRouteId { get; set; }
    public int SequenceNumber { get; set; }
    public int? CustomerId { get; set; }
    public int? SalesOrderId { get; set; }
    public required string DeliveryAddress { get; set; }
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Arrived, Delivered, Failed
    public DateTime? ArrivalTime { get; set; }
    public DateTime? DeliveryTime { get; set; }
    public string? DeliveryNotes { get; set; }
    public string? SignatureImagePath { get; set; }
    public string? PhotoEvidencePaths { get; set; } // JSON array of image paths
    public bool EvidenceCaptured { get; set; }
    
    // Navigation properties
    public DeliveryRoute? DeliveryRoute { get; set; }
    public Customer? Customer { get; set; }
    public SalesOrder? SalesOrder { get; set; }
}
