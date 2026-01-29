namespace AccountsPOC.Domain.Entities;

public class CustomField
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string EntityType { get; set; } // "BillOfMaterial", "SalesOrder", "SalesOrderItem", "SalesInvoiceItem", "StockItem", "Product"
    public required string FieldName { get; set; }
    public required string FieldLabel { get; set; }
    public required string FieldType { get; set; } // "Text", "Number", "Boolean", "Radio", "Checkbox", "Select", "TextArea"
    public string? Options { get; set; } // JSON array for Radio/Checkbox/Select options
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    
    // Navigation property
    public Tenant? Tenant { get; set; }
}
