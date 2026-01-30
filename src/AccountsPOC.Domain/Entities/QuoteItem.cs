namespace AccountsPOC.Domain.Entities;

public class QuoteItem
{
    public int Id { get; set; }
    public int QuoteId { get; set; }
    public int? ProductId { get; set; }
    
    public required string Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxRate { get; set; }
    public decimal LineTotal { get; set; }
    
    public string? ProductCode { get; set; }
    public string? Unit { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public Quote? Quote { get; set; }
    public Product? Product { get; set; }
}
