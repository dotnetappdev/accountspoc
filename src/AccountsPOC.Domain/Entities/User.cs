using Microsoft.AspNetCore.Identity;

namespace AccountsPOC.Domain.Entities;

public class User : IdentityUser<int>
{
    public int TenantId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    
    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
