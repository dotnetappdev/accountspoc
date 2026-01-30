namespace AccountsPOC.Domain.Entities;

public class SiteVisit
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string VisitNumber { get; set; }
    public DateTime VisitDate { get; set; }
    public DateTime? ScheduledStartTime { get; set; }
    public DateTime? ScheduledEndTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    
    // Site Information
    public string? SiteAddress { get; set; }
    public string? SiteCity { get; set; }
    public string? SitePostCode { get; set; }
    public string? SiteCountry { get; set; }
    public string? SiteContactName { get; set; }
    public string? SiteContactPhone { get; set; }
    
    // Customer Information
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    
    // Visit Details
    public required string VisitType { get; set; } // Initial, Progress, Completion, Inspection, Emergency
    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled
    public string? Purpose { get; set; }
    public string? Notes { get; set; }
    
    // Assignment
    public int? AssignedToUserId { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
    
    // Navigation properties
    public Customer? Customer { get; set; }
    public User? AssignedTo { get; set; }
    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    public ICollection<SiteVisitSignOff> SiteVisitSignOffs { get; set; } = new List<SiteVisitSignOff>();
}
