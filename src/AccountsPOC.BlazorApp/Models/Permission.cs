namespace AccountsPOC.BlazorApp.Models;

public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string Resource { get; set; } = "";
    public string Action { get; set; } = "";
    public DateTime CreatedDate { get; set; }
}
