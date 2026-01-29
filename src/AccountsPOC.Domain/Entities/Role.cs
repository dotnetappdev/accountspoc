using Microsoft.AspNetCore.Identity;

namespace AccountsPOC.Domain.Entities;

public class Role : IdentityRole<int>
{
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; } = false; // System roles like Support, Agent cannot be deleted
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
