namespace AccountsPOC.BlazorApp.Models;

public class BankAccount
{
    public int Id { get; set; }
    public required string AccountName { get; set; }
    public required string AccountNumber { get; set; }
    public required string BankName { get; set; }
    public string? BranchCode { get; set; }
    public decimal Balance { get; set; }
    public required string Currency { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public bool IsActive { get; set; } = true;
}
