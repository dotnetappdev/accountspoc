namespace AccountsPOC.BlazorApp.Models;

public class User
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public List<RoleInfo>? Roles { get; set; }
}

public class RoleInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}
