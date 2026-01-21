namespace AccountsPOC.Domain.Entities;

public class PriceListItem
{
    public int Id { get; set; }
    public int PriceListId { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public int? MinimumQuantity { get; set; }
    
    // Navigation properties
    public PriceList? PriceList { get; set; }
    public Product? Product { get; set; }
}
