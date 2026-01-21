namespace AccountsPOC.Domain.Entities;

public class FormSubmission
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int CustomFormId { get; set; }
    
    // JSON column for form answers
    public string AnswersJson { get; set; } = "{}";
    
    // File uploads stored as JSON array of file paths
    public string? FileUploadsJson { get; set; }
    
    public string? SubmittedBy { get; set; }
    public string? SubmitterEmail { get; set; }
    public string? SubmitterIpAddress { get; set; }
    
    public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public virtual CustomForm? CustomForm { get; set; }
}
