using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaxExportController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TaxExportController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Export data in HMRC-compatible CSV format for Making Tax Digital (MTD)
    /// </summary>
    [HttpGet("mtd-vat-return")]
    public async Task<IActionResult> ExportMTDVATReturn(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var invoices = await _context.SalesInvoices
            .Include(i => i.SalesOrder)
            .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
            .OrderBy(i => i.InvoiceDate)
            .ToListAsync();

        var csv = new StringBuilder();
        csv.AppendLine("Invoice Number,Invoice Date,Customer,Net Amount,VAT Amount,Gross Amount,VAT Rate");

        foreach (var invoice in invoices)
        {
            var vatRate = invoice.TaxAmount > 0 ? ((invoice.TaxAmount / invoice.SubTotal) * 100) : 0;
            csv.AppendLine($"\"{invoice.InvoiceNumber}\",\"{invoice.InvoiceDate:yyyy-MM-dd}\",\"{invoice.SalesOrder?.CustomerName}\",{invoice.SubTotal:F2},{invoice.TaxAmount:F2},{invoice.TotalAmount:F2},{vatRate:F2}");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"VAT_Return_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.csv");
    }

    /// <summary>
    /// Export detailed transaction list for HMRC audit
    /// </summary>
    [HttpGet("hmrc-audit-file")]
    public async Task<IActionResult> ExportHMRCAuditFile(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var invoices = await _context.SalesInvoices
            .Include(i => i.SalesOrder)
            .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
            .OrderBy(i => i.InvoiceDate)
            .ToListAsync();

        var csv = new StringBuilder();
        csv.AppendLine("Transaction Type,Date,Reference,Customer Name,Description,Net Amount,VAT Amount,Gross Amount,Status");

        foreach (var invoice in invoices)
        {
            csv.AppendLine($"Sales Invoice,\"{invoice.InvoiceDate:yyyy-MM-dd}\",\"{invoice.InvoiceNumber}\",\"{invoice.SalesOrder?.CustomerName}\",\"Invoice for order {invoice.SalesOrder?.OrderNumber}\",{invoice.SubTotal:F2},{invoice.TaxAmount:F2},{invoice.TotalAmount:F2},{invoice.Status}");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"HMRC_Audit_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.csv");
    }

    /// <summary>
    /// Export VAT summary report
    /// </summary>
    [HttpGet("vat-summary")]
    public async Task<IActionResult> GetVATSummary(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var invoices = await _context.SalesInvoices
            .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
            .ToListAsync();

        var summary = new
        {
            Period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
            TotalNet = invoices.Sum(i => i.SubTotal),
            TotalVAT = invoices.Sum(i => i.TaxAmount),
            TotalGross = invoices.Sum(i => i.TotalAmount),
            InvoiceCount = invoices.Count,
            VATRate = invoices.Sum(i => i.SubTotal) > 0 
                ? (invoices.Sum(i => i.TaxAmount) / invoices.Sum(i => i.SubTotal)) * 100 
                : 0
        };

        return Ok(summary);
    }
}
