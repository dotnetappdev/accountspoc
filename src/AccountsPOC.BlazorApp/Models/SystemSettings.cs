namespace AccountsPOC.BlazorApp.Models;

public class SystemSettings
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string DefaultCurrency { get; set; } = "USD";
    public string? CurrencySymbol { get; set; } = "$";
    public int CurrencyDecimalPlaces { get; set; } = 2;
    public string DateFormat { get; set; } = "MM/dd/yyyy";
    public string CompanyName { get; set; } = "";
    public string? CompanyLogo { get; set; }
    public string? CompanyAddress { get; set; }
    public string? TaxNumber { get; set; }
    public decimal DefaultTaxRate { get; set; }
    public string? EmailFromAddress { get; set; }
    public string? EmailFromName { get; set; }
    public bool EnablePaymentIntegration { get; set; }
    public string? StripePublishableKey { get; set; }
    public string? StripeSecretKey { get; set; }
    
    // PDF Generation Settings
    public string PdfPageSize { get; set; } = "A4";
    public string PdfOrientation { get; set; } = "Portrait";
    public decimal PdfMarginTop { get; set; } = 20;
    public decimal PdfMarginBottom { get; set; } = 20;
    public decimal PdfMarginLeft { get; set; } = 20;
    public decimal PdfMarginRight { get; set; } = 20;
    public bool PdfShowHeader { get; set; } = true;
    public bool PdfShowFooter { get; set; } = true;
    public bool PdfShowLogo { get; set; } = true;
    public string? PdfLogoPosition { get; set; } = "TopLeft";
    public int PdfLogoMaxWidth { get; set; } = 150;
    public int PdfLogoMaxHeight { get; set; } = 80;
    public string? PdfHeaderText { get; set; }
    public string? PdfFooterText { get; set; }
    public bool PdfShowPageNumbers { get; set; } = true;
    public string? PdfPageNumberFormat { get; set; } = "Page {0} of {1}";
    public string PdfFontFamily { get; set; } = "Arial";
    public int PdfFontSize { get; set; } = 10;
    public string PdfPrimaryColor { get; set; } = "#2563eb";
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
