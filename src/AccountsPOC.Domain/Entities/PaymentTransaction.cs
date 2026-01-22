namespace AccountsPOC.Domain.Entities;

public class PaymentTransaction
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int SalesInvoiceId { get; set; }
    public required string PaymentMethod { get; set; } // "Stripe", "ApplePay", "GooglePay"
    public required string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public required string Status { get; set; } // "Pending", "Completed", "Failed", "Refunded"
    public string? CustomerEmail { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedDate { get; set; }
    
    // Navigation properties
    public Tenant? Tenant { get; set; }
    public SalesInvoice? SalesInvoice { get; set; }
}
