namespace AccountsPOC.Domain.Entities;

public class Driver
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string DriverCode { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public string? MobilePhone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }
    public string? LicenseNumber { get; set; }
    public DateTime? LicenseExpiryDate { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? VehicleRegistration { get; set; }
    public string? VehicleType { get; set; }
    public string? VehicleCapacity { get; set; }
    public DateTime? EmploymentStartDate { get; set; }
    public DateTime? EmploymentEndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public ICollection<DeliveryRoute> DeliveryRoutes { get; set; } = new List<DeliveryRoute>();
}
