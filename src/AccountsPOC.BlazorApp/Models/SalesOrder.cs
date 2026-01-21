namespace AccountsPOC.BlazorApp.Models;

public class SalesOrder
{
    public int Id { get; set; }
    public required string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public required string CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Cancelled
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();
    public ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
}
