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
    
    // Delivery-specific details
    public string? DeliveryAddress { get; set; }
    public string? DeliveryCity { get; set; }
    public string? DeliveryPostCode { get; set; }
    public string? DeliveryCountry { get; set; }
    public string? DeliveryContactName { get; set; }
    public string? DeliveryContactPhone { get; set; }
    public string? DeliveryContactMobile { get; set; }
    public string? DeliveryInstructions { get; set; }
    public double? DeliveryLatitude { get; set; }
    public double? DeliveryLongitude { get; set; }
    public string? PreferredDeliveryTime { get; set; }
    public string? AccessCode { get; set; }
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public PriceList? DefaultPriceList { get; set; }
}
