using AccountsPOC.Domain.Entities;
using AccountsPOC.PdfGenerator.Infrastructure;
using AccountsPOC.PdfGenerator.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AccountsPOC.PdfGenerator.Templates;

public class PartialDispatchPdfTemplate : BasePdfTemplate
{
    private readonly PartialDispatch _dispatch;
    private readonly TenantBrandingInfo _branding;

    public PartialDispatchPdfTemplate(PartialDispatch dispatch, TenantBrandingInfo branding)
    {
        _dispatch = dispatch;
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
            column.Item().AlignCenter().Text("DISPATCH NOTE")
                .FontSize(24)
                .SemiBold()
                .FontColor(Colors.Blue.Medium);

            column.Item().PaddingVertical(10).LineHorizontal(1);

            // Dispatch information
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text($"Dispatch Number: {_dispatch.DispatchNumber}").SemiBold();
                    col.Item().Text($"Dispatch Date: {_dispatch.DispatchDate:dd/MM/yyyy}");
                    col.Item().Text($"Status: {_dispatch.Status}");
                    if (_dispatch.SalesOrder != null)
                        col.Item().Text($"Order Ref: {_dispatch.SalesOrder.OrderNumber}");
                    if (!string.IsNullOrEmpty(_dispatch.TrackingNumber))
                        col.Item().Text($"Tracking: {_dispatch.TrackingNumber}");
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Carrier Information").SemiBold().FontSize(12);
                    if (!string.IsNullOrEmpty(_dispatch.CarrierName))
                        col.Item().Text($"Carrier: {_dispatch.CarrierName}");
                    if (!string.IsNullOrEmpty(_dispatch.ShippingMethod))
                        col.Item().Text($"Method: {_dispatch.ShippingMethod}");
                    col.Item().Text($"Packages: {_dispatch.NumberOfPackages}");
                    if (_dispatch.Weight > 0)
                        col.Item().Text($"Weight: {_dispatch.Weight} {_dispatch.WeightUnit ?? "kg"}");
                });
            });

            // Delivery address
            if (!string.IsNullOrEmpty(_dispatch.DeliveryAddress))
            {
                column.Item().PaddingTop(10).Text("Delivery Address").SemiBold().FontSize(12);
                column.Item().Text(text =>
                {
                    if (!string.IsNullOrEmpty(_dispatch.DeliveryContactName))
                        text.Line(_dispatch.DeliveryContactName);
                    text.Line(_dispatch.DeliveryAddress);
                    if (!string.IsNullOrEmpty(_dispatch.DeliveryCity))
                        text.Line(_dispatch.DeliveryCity);
                    if (!string.IsNullOrEmpty(_dispatch.DeliveryPostCode))
                        text.Line(_dispatch.DeliveryPostCode);
                    if (!string.IsNullOrEmpty(_dispatch.DeliveryCountry))
                        text.Line(_dispatch.DeliveryCountry);
                    if (!string.IsNullOrEmpty(_dispatch.DeliveryContactPhone))
                        text.Line($"Phone: {_dispatch.DeliveryContactPhone}");
                });
            }

            column.Item().PaddingVertical(10).LineHorizontal(1);

            // Dispatched items table
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40); // Line #
                    columns.RelativeColumn(4); // Description
                    columns.RelativeColumn(1); // Qty Dispatched
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Line").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Description").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Qty Dispatched").SemiBold();
                });

                // Items
                var lineNumber = 1;
                foreach (var item in _dispatch.PartialDispatchItems)
                {
                    var orderItem = item.SalesOrderItem;
                    var description = orderItem?.IsFreeTextItem == true
                        ? orderItem.Description ?? "Free text item"
                        : orderItem?.Product?.ProductName ?? "Product";

                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5)
                        .Text(lineNumber.ToString());
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5)
                        .Text(description);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5)
                        .Text(item.QuantityDispatched.ToString());

                    lineNumber++;
                }
            });

            // Delivery information
            if (_dispatch.EstimatedDeliveryDate.HasValue || _dispatch.ActualDeliveryDate.HasValue)
            {
                column.Item().PaddingTop(10).Text("Delivery Information").SemiBold();
                column.Item().Text(text =>
                {
                    if (_dispatch.EstimatedDeliveryDate.HasValue)
                        text.Line($"Estimated Delivery: {_dispatch.EstimatedDeliveryDate.Value:dd/MM/yyyy}");
                    if (_dispatch.ActualDeliveryDate.HasValue)
                        text.Line($"Actual Delivery: {_dispatch.ActualDeliveryDate.Value:dd/MM/yyyy}");
                });
            }

            // Notes
            if (!string.IsNullOrEmpty(_dispatch.Notes))
            {
                column.Item().PaddingTop(15).Text("Notes:").SemiBold();
                column.Item().Text(_dispatch.Notes);
            }

            // Signature section
            column.Item().PaddingTop(30).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Received By:").SemiBold();
                    col.Item().PaddingTop(30).LineHorizontal(1);
                    col.Item().PaddingTop(5).Text("Signature").FontSize(9);
                });
                
                row.ConstantItem(50);
                
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Date:").SemiBold();
                    col.Item().PaddingTop(30).LineHorizontal(1);
                    col.Item().PaddingTop(5).Text("Date Received").FontSize(9);
                });
            });
        });
    }
}
