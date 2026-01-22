namespace AccountsPOC.Domain.Entities;

public class CustomFieldValue
{
    public int Id { get; set; }
    public int CustomFieldId { get; set; }
    public int EntityId { get; set; }
    public required string EntityType { get; set; }
    public string? FieldValue { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation property
    public CustomField? CustomField { get; set; }
}
