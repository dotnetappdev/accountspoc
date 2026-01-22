namespace AccountsPOC.Domain.Entities;

public class Parcel
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string ParcelBarcode { get; set; }
    public int? SalesOrderId { get; set; }
    public int? DeliveryStopId { get; set; }
    public int? ContainerId { get; set; } // Bag or Cage ID
    public string Status { get; set; } = "Pending"; // Pending, Scanned, InTransit, Delivered
    public DateTime? ScannedToVanAt { get; set; }
    public int? ScannedByDriverId { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public SalesOrder? SalesOrder { get; set; }
    public DeliveryStop? DeliveryStop { get; set; }
    public Container? Container { get; set; }
    public Driver? ScannedByDriver { get; set; }
}

public class Container
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string ContainerCode { get; set; }
    public string ContainerType { get; set; } = "Bag"; // Bag, Cage, Trolley
    public int? DeliveryRouteId { get; set; }
    public int? DriverId { get; set; }
    public string Status { get; set; } = "Available"; // Available, InUse, Full
    public int MaxCapacity { get; set; } = 50; // Maximum number of parcels
    public DateTime? LoadedAt { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public DeliveryRoute? DeliveryRoute { get; set; }
    public Driver? Driver { get; set; }
    public ICollection<Parcel> Parcels { get; set; } = new List<Parcel>();
}
