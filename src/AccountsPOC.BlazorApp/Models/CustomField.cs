namespace AccountsPOC.BlazorApp.Models;

public class CustomField
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string EntityType { get; set; } = "";
    public string FieldName { get; set; } = "";
    public string FieldLabel { get; set; } = "";
    public string FieldType { get; set; } = "Text";
    public string? Options { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
}
