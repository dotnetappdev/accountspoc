namespace AccountsPOC.Domain.Entities;

public class WorkOrderTask
{
    public int Id { get; set; }
    public int WorkOrderId { get; set; }
    
    public required string TaskName { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int? CompletedByUserId { get; set; }
    
    public int SortOrder { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    
    public string? Notes { get; set; }
    
    // Navigation properties
    public WorkOrder? WorkOrder { get; set; }
    public User? CompletedBy { get; set; }
}
