namespace AccountsPOC.BlazorApp.Models;

public class EmailTemplate
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string TemplateName { get; set; } = "";
    public string TemplateCode { get; set; } = "";
    public string Subject { get; set; } = "";
    public string BodyHtml { get; set; } = "";
    public string? TriggerEvent { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
