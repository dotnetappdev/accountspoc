namespace AccountsPOC.BlazorApp.Models;

public class BankAccount
{
    public int Id { get; set; }
    public required string AccountName { get; set; }
    public required string AccountNumber { get; set; }
    public required string BankName { get; set; }
    public string? BranchCode { get; set; }
    public string? IBAN { get; set; }
    public string? SortCode { get; set; }
    public string? AccountCode { get; set; }
    public string? IssueCode { get; set; }
    public string? ExpiryCode { get; set; }
    public string? StartCode { get; set; }
    public string? Extra { get; set; }
    public decimal Balance { get; set; }
    public required string Currency { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public bool IsActive { get; set; } = true;
}
