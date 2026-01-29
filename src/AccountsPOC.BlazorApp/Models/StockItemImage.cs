namespace AccountsPOC.BlazorApp.Models;

public class StockItemImage
{
    public int Id { get; set; }
    public int StockItemId { get; set; }
    public string ImageUrl { get; set; } = "";
    public string? ImageData { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsPrimaryImage { get; set; } = false;
    public DateTime CreatedDate { get; set; }
}
