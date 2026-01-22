namespace AccountsPOC.Domain.Entities;

public class ExchangeRate
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTime RateDate { get; set; }
    public DateTime LastUpdated { get; set; }
    public string Source { get; set; } = string.Empty; // "ECB", "Manual", etc.
    public bool IsActive { get; set; } = true;
}
