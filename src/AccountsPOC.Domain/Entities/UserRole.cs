using Microsoft.AspNetCore.Identity;

namespace AccountsPOC.Domain.Entities;

public class UserRole : IdentityUserRole<int>
{
    public DateTime AssignedDate { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
