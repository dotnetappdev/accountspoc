namespace AccountsPOC.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string CustomerCode { get; set; }
    public required string CompanyName { get; set; }
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public required string Address { get; set; }
    public string? City { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }
    public string? VATNumber { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public int? DefaultPriceListId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public PriceList? DefaultPriceList { get; set; }
}
