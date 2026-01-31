namespace AccountsPOC.BlazorApp.Models;

public class BOMImage
{
    public int Id { get; set; }
    public int BillOfMaterialId { get; set; }
    public required string ImageUrl { get; set; }
    public string? ImageData { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsPrimaryImage { get; set; } = false;
    public DateTime CreatedDate { get; set; }
    
    public BillOfMaterial? BillOfMaterial { get; set; }
}
