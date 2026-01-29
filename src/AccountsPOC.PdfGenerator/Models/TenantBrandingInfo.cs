namespace AccountsPOC.PdfGenerator.Models;

public class TenantBrandingInfo
{
    public string? TenantName { get; set; }
    public byte[]? LogoImage { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
}
