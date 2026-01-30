namespace AccountsPOC.BlazorApp.Models;

public class Product
{
    public int Id { get; set; }
    public required string ProductCode { get; set; }
    public required string ProductName { get; set; }
    public string? Description { get; set; }
    public string? LongDescription { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostPrice { get; set; }
    public int StockLevel { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Product Identification
    public string? Barcode { get; set; }
    public string? Category { get; set; }
    public string? ManufacturerPartNumber { get; set; }
    public string? InternalReference { get; set; }
    public string UnitOfMeasure { get; set; } = "Each";
    
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
    
    // Images
    public string? PrimaryImageUrl { get; set; }
    public List<string>? AdditionalImageUrls { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public ICollection<BillOfMaterial> BillOfMaterials { get; set; } = new List<BillOfMaterial>();
}
