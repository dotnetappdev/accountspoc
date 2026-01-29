namespace AccountsPOC.BlazorApp.Models;

public class Tenant
{
    public int Id { get; set; }
    public string TenantCode { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
