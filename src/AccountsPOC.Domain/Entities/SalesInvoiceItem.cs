namespace AccountsPOC.Domain.Entities;

public class SalesInvoiceItem
{
    public int Id { get; set; }
    public int SalesInvoiceId { get; set; }
    public int? ProductId { get; set; }
    
    // Free-text line item support
    public string? Description { get; set; }
    public bool IsFreeTextItem { get; set; }
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    
    // Line item details
    public int LineNumber { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public SalesInvoice? SalesInvoice { get; set; }
    public Product? Product { get; set; }
}
