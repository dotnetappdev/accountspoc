namespace AccountsPOC.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string ProductCode { get; set; }
    public required string ProductName { get; set; }
    public string? Description { get; set; }
    public string? LongDescription { get; set; }
    
    // Pricing
    public decimal UnitPrice { get; set; }
    public decimal CostPrice { get; set; }
    
    // Stock Control
    public int StockLevel { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public int QuantityAllocated { get; set; }
    public int QuantityAvailable => StockLevel - QuantityAllocated;
    
    // Sage 200 Product Fields
    public string? ProductGroup { get; set; }
    public string? Category { get; set; }
    public string UnitOfMeasure { get; set; } = "Each";
    public string? AlternativeUnitOfMeasure { get; set; }
    public decimal? UnitsPerPack { get; set; }
    
    // Identification
    public string? Barcode { get; set; }
    public string? ManufacturerPartNumber { get; set; }
    public string? InternalReference { get; set; }
    
    // Dimensions and Weight
    public decimal? Weight { get; set; }
    public string? WeightUOM { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public string? DimensionUOM { get; set; }
    
    // Tax and Accounting
    public bool TaxExempt { get; set; }
    public string? TaxCode { get; set; }
    public string? SalesAccountCode { get; set; }
    public string? PurchaseAccountCode { get; set; }
    
    // Warehouse and Location
    public int? DefaultWarehouseId { get; set; }
    public string? DefaultBinLocation { get; set; }
    
    // Product Type
    public string ProductType { get; set; } = "Stock"; // Stock, Service, NonStock, Labour
    public bool IsServiceItem { get; set; }
    public bool IsKitItem { get; set; }
    public bool AllowBackorder { get; set; } = true;
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool IsDiscontinued { get; set; }
    public DateTime? DiscontinuedDate { get; set; }
    
    // Notes
    public string? Notes { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Supplier information
    public int? PreferredSupplierId { get; set; }
    public decimal? SupplierUnitCost { get; set; }
    public string? SupplierPartNumber { get; set; }
    public int? SupplierLeadTimeDays { get; set; }
    
    // Navigation properties
    public Warehouse? DefaultWarehouse { get; set; }
    public ICollection<BillOfMaterial> BillOfMaterials { get; set; } = new List<BillOfMaterial>();
    public Supplier? PreferredSupplier { get; set; }
}
