using AccountsPOC.Domain.Entities;
using AccountsPOC.PdfGenerator.Infrastructure;
using AccountsPOC.PdfGenerator.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AccountsPOC.PdfGenerator.Templates;

public class SalesOrderPdfTemplate : BasePdfTemplate
{
    private readonly SalesOrder _salesOrder;
    private readonly TenantBrandingInfo _branding;

    public SalesOrderPdfTemplate(SalesOrder salesOrder, TenantBrandingInfo branding)
    {
        _salesOrder = salesOrder;
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
            column.Item().AlignCenter().Text("SALES ORDER")
                .FontSize(24)
                .SemiBold()
                .FontColor(Colors.Blue.Medium);

            column.Item().PaddingVertical(10).LineHorizontal(1);

            // Order information
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text($"Order Number: {_salesOrder.OrderNumber}").SemiBold();
                    col.Item().Text($"Order Date: {_salesOrder.OrderDate:dd/MM/yyyy}");
                    if (_salesOrder.RequiredDate.HasValue)
                        col.Item().Text($"Required Date: {_salesOrder.RequiredDate.Value:dd/MM/yyyy}");
                    col.Item().Text($"Status: {_salesOrder.Status}");
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Customer Information").SemiBold().FontSize(12);
                    col.Item().Text(_salesOrder.CustomerName);
                    if (!string.IsNullOrEmpty(_salesOrder.CustomerEmail))
                        col.Item().Text(_salesOrder.CustomerEmail);
                    if (!string.IsNullOrEmpty(_salesOrder.CustomerPhone))
                        col.Item().Text(_salesOrder.CustomerPhone);
                });
            });

            // Delivery information
            if (!string.IsNullOrEmpty(_salesOrder.DeliveryAddress))
            {
                column.Item().PaddingTop(10).Text("Delivery Address").SemiBold().FontSize(12);
                column.Item().Text(text =>
                {
                    text.Line(_salesOrder.DeliveryAddress);
                    if (!string.IsNullOrEmpty(_salesOrder.DeliveryCity))
                        text.Line(_salesOrder.DeliveryCity);
                    if (!string.IsNullOrEmpty(_salesOrder.DeliveryPostCode))
                        text.Line(_salesOrder.DeliveryPostCode);
                    if (!string.IsNullOrEmpty(_salesOrder.DeliveryCountry))
                        text.Line(_salesOrder.DeliveryCountry);
                });
            }

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
                    columns.RelativeColumn(1); // Total
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Line").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Description").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Qty").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Unit Price").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Total").SemiBold();
                });

                // Items
                var lineNumber = 1;
                foreach (var item in _salesOrder.SalesOrderItems.OrderBy(i => i.LineNumber))
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
                    text.Span($"£{_salesOrder.SubTotal:N2}");
                });
                
                if (_salesOrder.DiscountAmount > 0)
                {
                    col.Item().Text(text =>
                    {
                        text.Span("Discount: ").SemiBold();
                        text.Span($"-£{_salesOrder.DiscountAmount:N2}");
                    });
                }
                
                col.Item().Text(text =>
                {
                    text.Span("Tax: ").SemiBold();
                    text.Span($"£{_salesOrder.TaxAmount:N2}");
                });
                
                if (_salesOrder.ShippingCost > 0)
                {
                    col.Item().Text(text =>
                    {
                        text.Span("Shipping: ").SemiBold();
                        text.Span($"£{_salesOrder.ShippingCost:N2}");
                    });
                }
                
                col.Item().PaddingTop(5).Text(text =>
                {
                    text.Span("Total: ").FontSize(14).SemiBold();
                    text.Span($"£{_salesOrder.TotalAmount:N2}").FontSize(14).SemiBold();
                });
            });

            // Notes
            if (!string.IsNullOrEmpty(_salesOrder.CustomerNotes))
            {
                column.Item().PaddingTop(15).Text("Notes:").SemiBold();
                column.Item().Text(_salesOrder.CustomerNotes);
            }
        });
    }
}
