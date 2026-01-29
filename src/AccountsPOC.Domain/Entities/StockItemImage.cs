namespace AccountsPOC.Domain.Entities;

public class StockItemImage
{
    public int Id { get; set; }
    public int StockItemId { get; set; }
    public required string ImageUrl { get; set; }
    public string? ImageData { get; set; } // Base64 encoded image data for embedded storage
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsPrimaryImage { get; set; } = false;
    public DateTime CreatedDate { get; set; }
    
    // Navigation properties
    public StockItem StockItem { get; set; } = null!;
}
