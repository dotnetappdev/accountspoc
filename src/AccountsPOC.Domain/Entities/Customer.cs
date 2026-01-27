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
    public string? Mobile { get; set; }
    public string? Fax { get; set; }
    public string? Website { get; set; }
    
    // Address Information
    public required string Address { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? County { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }
    
    // Tax and Financial
    public string? VATNumber { get; set; }
    public string? TaxCode { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public string PaymentTerms { get; set; } = "Net 30"; // Net 30, Net 60, COD, etc.
    public int PaymentTermsDays { get; set; } = 30;
    public string? AccountNumber { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankSortCode { get; set; }
    
    // Sage 200 specific fields
    public string? CurrencyCode { get; set; }
    public int? SalesPersonId { get; set; }
    public string? CustomerGroup { get; set; }
    public string? IndustryType { get; set; }
    public bool OnHold { get; set; }
    public string? OnHoldReason { get; set; }
    public decimal DiscountPercentage { get; set; }
    public int? DefaultPriceListId { get; set; }
    public string? DeliveryTerms { get; set; }
    public int? DefaultWarehouseId { get; set; }
    public string? Notes { get; set; }
    
    // Delivery-specific details
    public string? DeliveryAddress { get; set; }
    public string? DeliveryAddressLine2 { get; set; }
    public string? DeliveryCity { get; set; }
    public string? DeliveryCounty { get; set; }
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
    public Warehouse? DefaultWarehouse { get; set; }
}
