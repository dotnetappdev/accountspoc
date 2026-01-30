namespace AccountsPOC.WebAPI.Models;

public class DatabaseConfiguration
{
    public string? ConnectionString { get; set; }
    public string DatabaseProvider { get; set; } = "SQLite";
    public DateTime ConfiguredDate { get; set; }
    public string ConfiguredBy { get; set; } = "Setup";
}
