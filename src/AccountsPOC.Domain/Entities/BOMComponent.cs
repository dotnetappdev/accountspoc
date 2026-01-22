namespace AccountsPOC.Domain.Entities;

public class BOMComponent
{
    public int Id { get; set; }
    public int BillOfMaterialId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    
    // Navigation properties
    public BillOfMaterial? BillOfMaterial { get; set; }
    public Product? Product { get; set; }
}
