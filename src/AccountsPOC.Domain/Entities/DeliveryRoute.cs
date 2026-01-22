namespace AccountsPOC.Domain.Entities;

// Enums for type safety
public static class DeliveryStatusType
{
    public const string Delivered = "Delivered";
    public const string SafePlace = "SafePlace";
    public const string LeftWithNeighbor = "LeftWithNeighbor";
}

public static class DeliveryStopStatus
{
    public const string Pending = "Pending";
    public const string Arrived = "Arrived";
    public const string Delivered = "Delivered";
    public const string Failed = "Failed";
}

public class DeliveryRoute
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string RouteNumber { get; set; }
    public string RouteName { get; set; } = string.Empty; // Display name for route
    public DateTime RouteDate { get; set; }
    public string Status { get; set; } = "Planned"; // Planned, InProgress, Completed
    public int? DriverId { get; set; }
    public string? VehicleRegistration { get; set; }
    public DateTime? StartedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? ActualStartTime { get; set; } // When driver actually started the route
    public DateTime? EstimatedEndTime { get; set; } // Estimated completion time
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
    
    // Delivery details for GPS and route optimization
    public string? DoorNumber { get; set; } // Door/apartment number for the delivery
    public int ParcelCount { get; set; } = 0; // Number of parcels to deliver at this stop
    public string? DeliveryStatus { get; set; } // "Delivered", "SafePlace", "LeftWithNeighbor"
    public string? NeighborDoorNumber { get; set; } // If left with neighbor, their door number
    public double? OptimizedDistance { get; set; } // Distance in km from previous stop (for optimization)
    
    // Safe place and access details (Amazon-style)
    public string? SafePlace { get; set; } // e.g., "Porch", "Rear Porch", "Garage", "Behind Gate"
    public string? DoorAccessCode { get; set; }
    public string? PostBoxCode { get; set; }
    public string? BuildingAccessInstructions { get; set; }
    
    // Age-restricted delivery
    public bool RequiresAgeVerification { get; set; } = false;
    public string? OTPCode { get; set; }
    public DateTime? OTPGeneratedAt { get; set; }
    public DateTime? OTPVerifiedAt { get; set; }
    public bool OTPVerified { get; set; } = false;
    
    // Navigation properties
    public DeliveryRoute? DeliveryRoute { get; set; }
    public Customer? Customer { get; set; }
    public SalesOrder? SalesOrder { get; set; }
}
