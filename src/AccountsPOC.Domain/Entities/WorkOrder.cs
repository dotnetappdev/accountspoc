namespace AccountsPOC.Domain.Entities;

public class WorkOrder
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string WorkOrderNumber { get; set; }
    public DateTime WorkOrderDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    
    // Customer Information
    public int? CustomerId { get; set; }
    public required string CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    
    // Job Site Information
    public string? SiteAddress { get; set; }
    public string? SiteCity { get; set; }
    public string? SitePostCode { get; set; }
    public string? SiteCountry { get; set; }
    public string? SiteContactName { get; set; }
    public string? SiteContactPhone { get; set; }
    
    // Work Order Details
    public required string Description { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Scheduled, InProgress, Completed, Cancelled, OnHold
    public string? Priority { get; set; } // Low, Normal, High, Urgent
    public int? AssignedToUserId { get; set; }
    
    // Related Orders
    public int? SalesOrderId { get; set; }
    public int? QuoteId { get; set; }
    public int? SiteVisitId { get; set; }
    
    // Costing
    public decimal EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    
    // Notes
    public string? InternalNotes { get; set; }
    public string? CustomerNotes { get; set; }
    public string? CompletionNotes { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
    
    // Navigation properties
    public Customer? Customer { get; set; }
    public User? AssignedTo { get; set; }
    public SalesOrder? SalesOrder { get; set; }
    public Quote? Quote { get; set; }
    public SiteVisit? SiteVisit { get; set; }
    public ICollection<WorkOrderTask> WorkOrderTasks { get; set; } = new List<WorkOrderTask>();
    public ICollection<SiteVisitSignOff> SiteVisitSignOffs { get; set; } = new List<SiteVisitSignOff>();
}
