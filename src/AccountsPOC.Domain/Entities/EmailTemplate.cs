namespace AccountsPOC.Domain.Entities;

public class EmailTemplate
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string TemplateName { get; set; }
    public required string TemplateCode { get; set; } // "INVOICE_CREATED", "PAYMENT_RECEIVED", etc.
    public required string Subject { get; set; }
    public required string BodyHtml { get; set; }
    public string? TriggerEvent { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation property
    public Tenant? Tenant { get; set; }
}
