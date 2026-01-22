namespace AccountsPOC.Domain.Entities;

public class CustomForm
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    
    // JSON column for form fields configuration
    public string FormFieldsJson { get; set; } = "[]";
    
    public bool IsActive { get; set; } = true;
    public bool AllowMultipleSubmissions { get; set; } = true;
    public bool RequireAuthentication { get; set; } = false;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
    
    // Navigation property
    public virtual ICollection<FormSubmission> Submissions { get; set; } = new List<FormSubmission>();
}
