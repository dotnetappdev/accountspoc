namespace AccountsPOC.Domain.Entities;

public class SystemSettings
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string DefaultCurrency { get; set; }
    public string? CurrencySymbol { get; set; }
    public int CurrencyDecimalPlaces { get; set; } = 2;
    public required string DateFormat { get; set; }
    public required string CompanyName { get; set; }
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
    public string PdfPageSize { get; set; } = "A4"; // A4, Letter, Legal
    public string PdfOrientation { get; set; } = "Portrait"; // Portrait, Landscape
    public decimal PdfMarginTop { get; set; } = 20; // mm
    public decimal PdfMarginBottom { get; set; } = 20; // mm
    public decimal PdfMarginLeft { get; set; } = 20; // mm
    public decimal PdfMarginRight { get; set; } = 20; // mm
    public bool PdfShowHeader { get; set; } = true;
    public bool PdfShowFooter { get; set; } = true;
    public bool PdfShowLogo { get; set; } = true;
    public string? PdfLogoPosition { get; set; } = "TopLeft"; // TopLeft, TopCenter, TopRight
    public int PdfLogoMaxWidth { get; set; } = 150; // pixels
    public int PdfLogoMaxHeight { get; set; } = 80; // pixels
    public string? PdfHeaderText { get; set; }
    public string? PdfFooterText { get; set; }
    public bool PdfShowPageNumbers { get; set; } = true;
    public string? PdfPageNumberFormat { get; set; } = "Page {0} of {1}";
    public string PdfFontFamily { get; set; } = "Arial";
    public int PdfFontSize { get; set; } = 10;
    public string PdfPrimaryColor { get; set; } = "#2563eb"; // Hex color for accents
    
    // Pagination Settings
    public int DefaultPageSize { get; set; } = 25; // Default number of items per page
    public int MaxPageSize { get; set; } = 100; // Maximum items per page
    public string PaginationSizes { get; set; } = "10,25,50,100"; // Available page sizes for dropdown
    public bool ShowPaginationInfo { get; set; } = true; // Show "Showing X to Y of Z entries"
    public bool ShowPageNumbers { get; set; } = true; // Show numbered page buttons
    public int MaxPageNumbers { get; set; } = 5; // Maximum page number buttons to display
    
    // File Upload Settings
    public bool AllowMultipleFileUploads { get; set; } = true;
    public int MaxFileUploadCount { get; set; } = 10; // Maximum files in a single upload
    public long MaxFileUploadSizeMB { get; set; } = 10; // Maximum file size in MB
    public string AllowedFileExtensions { get; set; } = ".pdf,.doc,.docx,.xls,.xlsx,.jpg,.jpeg,.png,.gif"; // Comma-separated
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    // Navigation property
    public Tenant? Tenant { get; set; }
}
