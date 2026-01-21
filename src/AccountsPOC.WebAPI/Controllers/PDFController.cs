using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PDFController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PDFController(ApplicationDbContext context)
    {
        _context = context;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [HttpGet("invoice/{invoiceId}")]
    public async Task<IActionResult> GenerateInvoice(int invoiceId)
    {
        var invoice = await _context.SalesInvoices
            .Include(i => i.SalesOrder)
            .ThenInclude(so => so!.SalesOrderItems)
            .ThenInclude(soi => soi.Product)
            .FirstOrDefaultAsync(i => i.Id == invoiceId);

        if (invoice == null || invoice.SalesOrder == null)
        {
            return NotFound("Invoice not found");
        }

        var pdfBytes = GenerateInvoicePDF(invoice);
        
        return File(pdfBytes, "application/pdf", $"Invoice-{invoice.InvoiceNumber}.pdf");
    }

    private byte[] GenerateInvoicePDF(Domain.Entities.SalesInvoice invoice)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(ComposeHeader);
                page.Content().Element(content => ComposeContent(content, invoice));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("ACCOUNTS POC").FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                column.Item().Text("123 Business Street").FontSize(9);
                column.Item().Text("Business City, BC 12345").FontSize(9);
                column.Item().Text("Email: info@accountspoc.com").FontSize(9);
                column.Item().Text("Phone: +1 (555) 123-4567").FontSize(9);
            });

            row.RelativeItem().Column(column =>
            {
                column.Item().AlignRight().Text("INVOICE").FontSize(20).Bold();
            });
        });
    }

    private void ComposeContent(IContainer container, Domain.Entities.SalesInvoice invoice)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Spacing(10);

            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Bill To:").Bold().FontSize(11);
                    col.Item().PaddingTop(5).Text(invoice.SalesOrder?.CustomerName ?? "N/A").FontSize(10);
                    if (!string.IsNullOrEmpty(invoice.SalesOrder?.CustomerEmail))
                        col.Item().Text($"Email: {invoice.SalesOrder.CustomerEmail}").FontSize(9);
                    if (!string.IsNullOrEmpty(invoice.SalesOrder?.CustomerPhone))
                        col.Item().Text($"Phone: {invoice.SalesOrder.CustomerPhone}").FontSize(9);
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignRight().Text($"Invoice #: {invoice.InvoiceNumber}").Bold();
                    col.Item().AlignRight().Text($"Invoice Date: {invoice.InvoiceDate:MM/dd/yyyy}");
                    col.Item().AlignRight().Text($"Due Date: {(invoice.DueDate.HasValue ? invoice.DueDate.Value.ToString("MM/dd/yyyy") : "N/A")}");
                    col.Item().AlignRight().Text($"Order #: {invoice.SalesOrder?.OrderNumber ?? "N/A"}");
                    col.Item().AlignRight().Text($"Status: {invoice.Status}").FontColor(GetStatusColor(invoice.Status));
                });
            });

            column.Item().PaddingTop(20).Element(content => ComposeLineItemsTable(content, invoice));

            column.Item().PaddingTop(20).AlignRight().Column(col =>
            {
                col.Spacing(5);
                
                col.Item().Row(row =>
                {
                    row.AutoItem().Width(150).Text("Subtotal:").FontSize(11);
                    row.AutoItem().Width(100).AlignRight().Text($"${invoice.SubTotal:N2}").FontSize(11);
                });

                col.Item().Row(row =>
                {
                    row.AutoItem().Width(150).Text("Tax:").FontSize(11);
                    row.AutoItem().Width(100).AlignRight().Text($"${invoice.TaxAmount:N2}").FontSize(11);
                });

                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);

                col.Item().Row(row =>
                {
                    row.AutoItem().Width(150).Text("Total Amount:").Bold().FontSize(12);
                    row.AutoItem().Width(100).AlignRight().Text($"${invoice.TotalAmount:N2}").Bold().FontSize(12).FontColor(Colors.Blue.Darken2);
                });
            });
        });
    }

    private void ComposeLineItemsTable(IContainer container, Domain.Entities.SalesInvoice invoice)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(40);
                columns.RelativeColumn(3);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("#").Bold();
                header.Cell().Element(CellStyle).Text("Description").Bold();
                header.Cell().Element(CellStyle).AlignRight().Text("Quantity").Bold();
                header.Cell().Element(CellStyle).AlignRight().Text("Unit Price").Bold();
                header.Cell().Element(CellStyle).AlignRight().Text("Total").Bold();

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.FontSize(10))
                        .PaddingVertical(5)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Medium);
                }
            });

            var items = invoice.SalesOrder?.SalesOrderItems ?? new List<Domain.Entities.SalesOrderItem>();
            int index = 1;
            
            foreach (var item in items)
            {
                table.Cell().Element(CellStyle).Text(index.ToString());
                table.Cell().Element(CellStyle).Text(item.Product?.ProductName ?? "Unknown Product");
                table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString());
                table.Cell().Element(CellStyle).AlignRight().Text($"${item.UnitPrice:N2}");
                table.Cell().Element(CellStyle).AlignRight().Text($"${item.TotalPrice:N2}");

                index++;

                static IContainer CellStyle(IContainer container)
                {
                    return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                }
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Column(column =>
        {
            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
            column.Item().PaddingTop(10).Text("Payment Terms:").Bold().FontSize(10);
            column.Item().Text("Payment is due within 30 days of invoice date.").FontSize(9);
            column.Item().Text("Please include invoice number with payment.").FontSize(9);
            column.Item().PaddingTop(5).Text("Thank you for your business!").FontSize(9).Italic();
        });
    }

    private string GetStatusColor(string status)
    {
        return status.ToLower() switch
        {
            "paid" => "#22C55E",
            "unpaid" => "#F59E0B",
            "overdue" => "#EF4444",
            "cancelled" => "#6B7280",
            _ => "#000000"
        };
    }
}
