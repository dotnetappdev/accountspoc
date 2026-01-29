using AccountsPOC.Domain.Entities;
using AccountsPOC.PdfGenerator.Infrastructure;
using AccountsPOC.PdfGenerator.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AccountsPOC.PdfGenerator.Templates;

public class SalesInvoicePdfTemplate : BasePdfTemplate
{
    private readonly SalesInvoice _salesInvoice;
    private readonly TenantBrandingInfo _branding;

    public SalesInvoicePdfTemplate(SalesInvoice salesInvoice, TenantBrandingInfo branding)
    {
        _salesInvoice = salesInvoice;
        _branding = branding;
        
        TenantName = branding.TenantName;
        TenantLogo = branding.LogoImage;
        TenantAddress = branding.Address;
        TenantPhone = branding.Phone;
        TenantEmail = branding.Email;
    }

    public override void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(50);
            page.Size(PageSizes.A4);

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);

            // Document title
            column.Item().AlignCenter().Text("SALES INVOICE")
                .FontSize(24)
                .SemiBold()
                .FontColor(Colors.Blue.Medium);

            column.Item().PaddingVertical(10).LineHorizontal(1);

            // Invoice information
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text($"Invoice Number: {_salesInvoice.InvoiceNumber}").SemiBold();
                    col.Item().Text($"Invoice Date: {_salesInvoice.InvoiceDate:dd/MM/yyyy}");
                    if (_salesInvoice.DueDate.HasValue)
                        col.Item().Text($"Due Date: {_salesInvoice.DueDate.Value:dd/MM/yyyy}");
                    col.Item().Text($"Status: {_salesInvoice.Status}");
                    if (_salesInvoice.SalesOrder != null)
                        col.Item().Text($"Order Ref: {_salesInvoice.SalesOrder.OrderNumber}");
                });

                if (_salesInvoice.SalesOrder != null)
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Customer Information").SemiBold().FontSize(12);
                        col.Item().Text(_salesInvoice.SalesOrder.CustomerName);
                        if (!string.IsNullOrEmpty(_salesInvoice.SalesOrder.CustomerEmail))
                            col.Item().Text(_salesInvoice.SalesOrder.CustomerEmail);
                        if (!string.IsNullOrEmpty(_salesInvoice.SalesOrder.CustomerPhone))
                            col.Item().Text(_salesInvoice.SalesOrder.CustomerPhone);
                    });
                }
            });

            column.Item().PaddingVertical(10).LineHorizontal(1);

            // Line items table
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40); // Line #
                    columns.RelativeColumn(3); // Description
                    columns.RelativeColumn(1); // Quantity
                    columns.RelativeColumn(1); // Unit Price
                    columns.RelativeColumn(1); // Tax
                    columns.RelativeColumn(1); // Total
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Line").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Description").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Qty").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Unit Price").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Tax").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Total").SemiBold();
                });

                // Items
                var lineNumber = 1;
                foreach (var item in _salesInvoice.SalesInvoiceItems.OrderBy(i => i.LineNumber))
                {
                    var description = item.IsFreeTextItem 
                        ? item.Description ?? "Free text item"
                        : item.Product?.ProductName ?? "Product";

                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5)
                        .Text(item.LineNumber > 0 ? item.LineNumber.ToString() : lineNumber.ToString());
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5)
                        .Text(description);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5)
                        .Text(item.Quantity.ToString());
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5)
                        .Text($"£{item.UnitPrice:N2}");
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5)
                        .Text($"£{item.TaxAmount:N2}");
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5)
                        .Text($"£{item.TotalPrice:N2}");

                    lineNumber++;
                }
            });

            // Totals
            column.Item().PaddingTop(10).AlignRight().Column(col =>
            {
                col.Item().Text(text =>
                {
                    text.Span("Subtotal: ").SemiBold();
                    text.Span($"£{_salesInvoice.SubTotal:N2}");
                });
                
                col.Item().Text(text =>
                {
                    text.Span("Tax: ").SemiBold();
                    text.Span($"£{_salesInvoice.TaxAmount:N2}");
                });
                
                col.Item().PaddingTop(5).Text(text =>
                {
                    text.Span("Total Amount Due: ").FontSize(14).SemiBold();
                    text.Span($"£{_salesInvoice.TotalAmount:N2}").FontSize(14).SemiBold();
                });
            });

            // Payment information
            column.Item().PaddingTop(15).Text(text =>
            {
                text.Span("Please make payment within ");
                if (_salesInvoice.DueDate.HasValue)
                {
                    var daysUntilDue = (_salesInvoice.DueDate.Value - _salesInvoice.InvoiceDate).Days;
                    text.Span($"{daysUntilDue} days. ");
                }
                text.Span("Thank you for your business.");
            });
        });
    }
}
