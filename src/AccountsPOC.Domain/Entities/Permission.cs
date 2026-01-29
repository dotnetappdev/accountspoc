namespace AccountsPOC.Domain.Entities;

public class Permission
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Resource { get; set; } // e.g., "Tenant", "Customer", "StockItem"
    public required string Action { get; set; } // e.g., "Create", "Read", "Update", "Delete", "Manage"
    public DateTime CreatedDate { get; set; }
    
    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
