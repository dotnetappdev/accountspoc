using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using AccountsPOC.PdfGenerator.Models;
using AccountsPOC.PdfGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesInvoicesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPdfGeneratorService _pdfService;

    public SalesInvoicesController(ApplicationDbContext context, IPdfGeneratorService pdfService)
    {
        _context = context;
        _pdfService = pdfService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SalesInvoice>>> GetSalesInvoices()
    {
        return await _context.SalesInvoices
            .Include(i => i.SalesOrder)
            .Include(i => i.SalesInvoiceItems)
                .ThenInclude(ii => ii.Product)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SalesInvoice>> GetSalesInvoice(int id)
    {
        var salesInvoice = await _context.SalesInvoices
            .Include(i => i.SalesOrder)
            .Include(i => i.SalesInvoiceItems)
                .ThenInclude(ii => ii.Product)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (salesInvoice == null)
        {
            return NotFound();
        }

        return salesInvoice;
    }

    [HttpPost]
    public async Task<ActionResult<SalesInvoice>> PostSalesInvoice(SalesInvoice salesInvoice)
    {
        salesInvoice.CreatedDate = DateTime.UtcNow;
        salesInvoice.InvoiceDate = DateTime.UtcNow;
        salesInvoice.DueDate = DateTime.UtcNow.AddDays(30);
        
        _context.SalesInvoices.Add(salesInvoice);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSalesInvoice), new { id = salesInvoice.Id }, salesInvoice);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutSalesInvoice(int id, SalesInvoice salesInvoice)
    {
        if (id != salesInvoice.Id)
        {
            return BadRequest();
        }

        salesInvoice.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(salesInvoice).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SalesInvoiceExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSalesInvoice(int id)
    {
        var salesInvoice = await _context.SalesInvoices.FindAsync(id);
        if (salesInvoice == null)
        {
            return NotFound();
        }

        _context.SalesInvoices.Remove(salesInvoice);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SalesInvoiceExists(int id)
    {
        return _context.SalesInvoices.Any(e => e.Id == id);
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetSalesInvoicePdf(int id)
    {
        var salesInvoice = await _context.SalesInvoices
            .Include(i => i.SalesOrder)
            .Include(i => i.SalesInvoiceItems)
                .ThenInclude(ii => ii.Product)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (salesInvoice == null)
        {
            return NotFound();
        }

        // Get tenant branding
        var branding = await GetTenantBranding(salesInvoice.TenantId);

        // Generate PDF
        var pdfBytes = _pdfService.GenerateSalesInvoicePdf(salesInvoice, branding);

        return File(pdfBytes, "application/pdf", $"Invoice_{salesInvoice.InvoiceNumber}.pdf");
    }

    private async Task<TenantBrandingInfo> GetTenantBranding(int tenantId)
    {
        var tenant = await _context.Tenants.FindAsync(tenantId);
        var logo = await _context.BrandingAssets
            .Where(b => b.TenantId == tenantId && b.AssetType == "Logo" && b.IsActive)
            .FirstOrDefaultAsync();

        byte[]? logoBytes = null;
        if (logo?.ImageData != null && logo.ImageData.StartsWith("data:image"))
        {
            // Extract base64 from data URL safely
            var parts = logo.ImageData.Split(',');
            if (parts.Length >= 2)
            {
                try
                {
                    logoBytes = Convert.FromBase64String(parts[1]);
                }
                catch (FormatException)
                {
                    // Invalid base64, log and continue without logo
                    Console.WriteLine($"Invalid base64 image data for tenant {tenantId}");
                }
            }
        }

        return new TenantBrandingInfo
        {
            TenantName = tenant?.CompanyName ?? "Company Name",
            LogoImage = logoBytes,
            Address = tenant != null ? $"{tenant.CompanyName} Address" : "Company Address",
            Phone = tenant?.ContactPhone ?? "+44 1234 567890",
            Email = tenant?.ContactEmail ?? "info@company.com"
        };
    }
}
