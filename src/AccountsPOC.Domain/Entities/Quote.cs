namespace AccountsPOC.Domain.Entities;

public class Quote
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string QuoteNumber { get; set; }
    public DateTime QuoteDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    
    // Customer Information
    public int? CustomerId { get; set; }
    public required string CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerReference { get; set; }
    
    // Quote Details
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal ExchangeRate { get; set; } = 1.0m;
    
    // Status
    public string Status { get; set; } = "Draft"; // Draft, Sent, Accepted, Rejected, Expired, Converted
    public DateTime? SentDate { get; set; }
    public DateTime? AcceptedDate { get; set; }
    public int? ConvertedToOrderId { get; set; }
    
    // Additional Information
    public string? InternalNotes { get; set; }
    public string? CustomerNotes { get; set; }
    public string? Terms { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
    
    // Navigation properties
    public Customer? Customer { get; set; }
    public ICollection<QuoteItem> QuoteItems { get; set; } = new List<QuoteItem>();
}
