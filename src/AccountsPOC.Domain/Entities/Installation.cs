namespace AccountsPOC.Domain.Entities;

public class Installation
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int LicenseId { get; set; }
    public required string InstallationKey { get; set; }
    public required string MachineName { get; set; }
    public string? MachineIdentifier { get; set; } // Hardware ID or similar
    public string? IpAddress { get; set; }
    public string? Version { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ActivationDate { get; set; }
    public DateTime? DeactivationDate { get; set; }
    public DateTime? LastHeartbeat { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public License License { get; set; } = null!;
}
