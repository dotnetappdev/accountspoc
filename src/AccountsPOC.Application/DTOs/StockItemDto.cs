namespace AccountsPOC.Application.DTOs;

/// <summary>
/// Data Transfer Object for Stock Item - based on Sage 200 stock item fields
/// </summary>
public class StockItemDto
{
    public int Id { get; set; }
    public string StockCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LongDescription { get; set; }
    
    // Product and categorization
    public int ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string? Category { get; set; }
    
    // Warehouse and location
    public int? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? BinLocation { get; set; }
    
    // Pricing
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    
    // Stock quantities
    public int QuantityOnHand { get; set; }
    public int QuantityAllocated { get; set; }
    public int QuantityAvailable { get; set; }
    public int QuantityOnOrder { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
    
    // Units
    public string UnitOfMeasure { get; set; } = "Each";
    public string? AlternativeUnitOfMeasure { get; set; }
    public decimal? UnitsPerPack { get; set; }
    
    // Supplier information
    public int? DefaultSupplierId { get; set; }
    public string? DefaultSupplierName { get; set; }
    public string? SupplierPartNumber { get; set; }
    public decimal? SupplierLeadTimeDays { get; set; }
    
    // Product identification
    public string? Barcode { get; set; }
    public string? ManufacturerPartNumber { get; set; }
    public string? InternalReference { get; set; }
    
    // Dimensions and weight (for shipping)
    public decimal? Weight { get; set; }
    public string? WeightUOM { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public string? DimensionUOM { get; set; }
    
    // Tax and accounting
    public bool TaxExempt { get; set; }
    public string? TaxCode { get; set; }
    public string? AccountCode { get; set; }
    
    // Age restriction (existing)
    public bool IsAgeRestricted { get; set; }
    public int? MinimumAge { get; set; }
    public bool RequiresOTPVerification { get; set; }
    
    // Status and tracking
    public bool IsActive { get; set; } = true;
    public bool IsDiscontinued { get; set; }
    public DateTime? DiscontinuedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Notes
    public string? Notes { get; set; }
}
