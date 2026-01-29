namespace AccountsPOC.BlazorApp.Models;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public List<PermissionInfo>? Permissions { get; set; }
}

public class PermissionInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Resource { get; set; } = "";
    public string Action { get; set; } = "";
}
