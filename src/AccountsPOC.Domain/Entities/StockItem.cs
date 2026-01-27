namespace AccountsPOC.Domain.Entities;

public class StockItem
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string StockCode { get; set; }
    public required string Description { get; set; }
    public string? LongDescription { get; set; }
    public int ProductId { get; set; }
    public int? WarehouseId { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int QuantityOnHand { get; set; }
    public int QuantityAllocated { get; set; }
    public int QuantityAvailable => QuantityOnHand - QuantityAllocated;
    public int QuantityOnOrder { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public string? BinLocation { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Sage 200 Additional Fields
    // Units of Measure
    public string UnitOfMeasure { get; set; } = "Each";
    public string? AlternativeUnitOfMeasure { get; set; }
    public decimal? UnitsPerPack { get; set; }
    
    // Supplier Information
    public int? DefaultSupplierId { get; set; }
    public string? SupplierPartNumber { get; set; }
    public decimal? SupplierLeadTimeDays { get; set; }
    
    // Product Identification
    public string? Barcode { get; set; }
    public string? ManufacturerPartNumber { get; set; }
    public string? InternalReference { get; set; }
    public string? Category { get; set; }
    
    // Dimensions and Weight (for shipping)
    public decimal? Weight { get; set; }
    public string? WeightUOM { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public string? DimensionUOM { get; set; }
    
    // Tax and Accounting
    public bool TaxExempt { get; set; }
    public string? TaxCode { get; set; }
    public string? AccountCode { get; set; }
    
    // Status
    public bool IsDiscontinued { get; set; }
    public DateTime? DiscontinuedDate { get; set; }
    
    // Notes
    public string? Notes { get; set; }
    
    // Age-restricted product fields
    public bool IsAgeRestricted { get; set; } = false;
    public int? MinimumAge { get; set; } = 18;
    public bool RequiresOTPVerification { get; set; } = false;
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public Product? Product { get; set; }
    public Warehouse? Warehouse { get; set; }
    public Supplier? DefaultSupplier { get; set; }
}
