namespace AccountsPOC.Domain.Entities;

public class SiteVisitSignOff
{
    public int Id { get; set; }
    public int WorkOrderId { get; set; }
    
    public DateTime VisitDate { get; set; }
    public required string VisitType { get; set; } // Initial, Progress, Completion, Inspection
    
    // Sign Off Details
    public required string SignedByName { get; set; }
    public string? SignedByTitle { get; set; }
    public DateTime SignedDate { get; set; }
    public string? SignatureImagePath { get; set; }
    
    // Visit Summary
    public string? WorkCompleted { get; set; }
    public string? IssuesIdentified { get; set; }
    public string? NextSteps { get; set; }
    public string? CustomerComments { get; set; }
    
    // Photos/Documentation
    public string? PhotoPaths { get; set; } // JSON array of photo paths
    
    // Rating
    public int? CustomerSatisfactionRating { get; set; } // 1-5 stars
    
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    
    // Navigation properties
    public WorkOrder? WorkOrder { get; set; }
}
