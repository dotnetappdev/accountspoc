namespace AccountsPOC.BlazorApp.Models;

public class BillOfMaterial
{
    public int Id { get; set; }
    public required string BOMNumber { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal EstimatedCost { get; set; }
    
    public string? Version { get; set; }
    public string? Revision { get; set; }
    public string BOMType { get; set; } = "Production";
    public string Status { get; set; } = "Active";
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    
    public decimal? SetupTime { get; set; }
    public decimal? ProductionTime { get; set; }
    public string? TimeUOM { get; set; }
    public decimal? ScrapPercentage { get; set; }
    public decimal? YieldPercentage { get; set; }
    public int? DefaultWarehouseId { get; set; }
    
    public decimal? LabourCost { get; set; }
    public decimal? OverheadCost { get; set; }
    public decimal? MaterialCost { get; set; }
    public decimal? TotalCost { get; set; }
    
    public bool CanBeLinkedToSalesOrder { get; set; } = true;
    public bool AutoCreateFromSalesOrder { get; set; }
    public bool AllowPartialKitting { get; set; } = true;
    public int? MinimumBatchSize { get; set; }
    public int? MaximumBatchSize { get; set; }
    
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    public Product? Product { get; set; }
    public ICollection<BOMComponent> Components { get; set; } = new List<BOMComponent>();
    public ICollection<BOMImage> Images { get; set; } = new List<BOMImage>();
    public ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();
}
