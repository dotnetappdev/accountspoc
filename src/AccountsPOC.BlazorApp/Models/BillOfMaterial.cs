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
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public Product? Product { get; set; }
    public ICollection<BOMComponent> Components { get; set; } = new List<BOMComponent>();
    public ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();
}
