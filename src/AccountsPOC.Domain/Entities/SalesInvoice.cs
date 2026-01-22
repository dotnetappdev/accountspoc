namespace AccountsPOC.Domain.Entities;

public class SalesInvoice
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public int SalesOrderId { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Unpaid"; // Unpaid, Paid, Overdue, Cancelled
    public DateTime? DueDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation property
    public SalesOrder? SalesOrder { get; set; }
}
