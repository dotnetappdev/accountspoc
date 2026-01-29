using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AccountsPOC.PdfGenerator.Infrastructure;

/// <summary>
/// Base class for all PDF document templates with tenant branding support
/// </summary>
public abstract class BasePdfTemplate : IDocument
{
    protected string? TenantName { get; set; }
    protected byte[]? TenantLogo { get; set; }
    protected string? TenantAddress { get; set; }
    protected string? TenantPhone { get; set; }
    protected string? TenantEmail { get; set; }
    
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    
    public abstract void Compose(IDocumentContainer container);
    
    protected void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                if (TenantLogo != null && TenantLogo.Length > 0)
                {
                    column.Item().Image(TenantLogo).FitWidth();
                }
                
                if (!string.IsNullOrEmpty(TenantName))
                {
                    column.Item().Text(TenantName)
                        .FontSize(20)
                        .SemiBold()
                        .FontColor(Colors.Blue.Medium);
                }
            });
            
            row.RelativeItem().Column(column =>
            {
                column.Item().AlignRight().Text(text =>
                {
                    if (!string.IsNullOrEmpty(TenantAddress))
                    {
                        text.Line(TenantAddress);
                    }
                    if (!string.IsNullOrEmpty(TenantPhone))
                    {
                        text.Line($"Phone: {TenantPhone}");
                    }
                    if (!string.IsNullOrEmpty(TenantEmail))
                    {
                        text.Line($"Email: {TenantEmail}");
                    }
                    text.Span("").FontSize(9);
                });
            });
        });
    }
    
    protected void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.CurrentPageNumber();
            text.Span(" / ");
            text.TotalPages();
        });
    }
}
