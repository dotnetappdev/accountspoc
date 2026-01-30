using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using AccountsPOC.PdfGenerator.Models;
using AccountsPOC.PdfGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesOrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPdfGeneratorService _pdfService;

    public SalesOrdersController(ApplicationDbContext context, IPdfGeneratorService pdfService)
    {
        _context = context;
        _pdfService = pdfService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SalesOrder>>> GetSalesOrders()
    {
        return await _context.SalesOrders
            .Include(o => o.SalesOrderItems)
            .ThenInclude(i => i.Product)
            .Include(o => o.SalesOrderItems)
            .ThenInclude(i => i.BillOfMaterial)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SalesOrder>> GetSalesOrder(int id)
    {
        var salesOrder = await _context.SalesOrders
            .Include(o => o.SalesOrderItems)
            .ThenInclude(i => i.Product)
            .Include(o => o.SalesOrderItems)
            .ThenInclude(i => i.BillOfMaterial)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (salesOrder == null)
        {
            return NotFound();
        }

        return salesOrder;
    }

    [HttpPost]
    public async Task<ActionResult<SalesOrder>> PostSalesOrder(SalesOrder salesOrder)
    {
        salesOrder.CreatedDate = DateTime.UtcNow;
        salesOrder.OrderDate = DateTime.UtcNow;
        
        foreach (var item in salesOrder.SalesOrderItems)
        {
            item.TotalPrice = item.Quantity * item.UnitPrice;
        }
        
        salesOrder.TotalAmount = salesOrder.SalesOrderItems.Sum(i => i.TotalPrice);
        
        _context.SalesOrders.Add(salesOrder);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSalesOrder), new { id = salesOrder.Id }, salesOrder);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutSalesOrder(int id, SalesOrder salesOrder)
    {
        if (id != salesOrder.Id)
        {
            return BadRequest();
        }

        salesOrder.LastModifiedDate = DateTime.UtcNow;
        
        foreach (var item in salesOrder.SalesOrderItems)
        {
            item.TotalPrice = item.Quantity * item.UnitPrice;
        }
        
        salesOrder.TotalAmount = salesOrder.SalesOrderItems.Sum(i => i.TotalPrice);
        
        // Get existing order with items
        var existingOrder = await _context.SalesOrders
            .Include(o => o.SalesOrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);
            
        if (existingOrder == null)
        {
            return NotFound();
        }
        
        // Update order properties
        existingOrder.OrderNumber = salesOrder.OrderNumber;
        existingOrder.OrderDate = salesOrder.OrderDate;
        existingOrder.CustomerName = salesOrder.CustomerName;
        existingOrder.CustomerEmail = salesOrder.CustomerEmail;
        existingOrder.CustomerPhone = salesOrder.CustomerPhone;
        existingOrder.TotalAmount = salesOrder.TotalAmount;
        existingOrder.Status = salesOrder.Status;
        existingOrder.LastModifiedDate = salesOrder.LastModifiedDate;
        
        // Remove old items
        _context.SalesOrderItems.RemoveRange(existingOrder.SalesOrderItems);
        
        // Add new items
        foreach (var item in salesOrder.SalesOrderItems)
        {
            existingOrder.SalesOrderItems.Add(new SalesOrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice,
                BillOfMaterialId = item.BillOfMaterialId
            });
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SalesOrderExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSalesOrder(int id)
    {
        var salesOrder = await _context.SalesOrders.FindAsync(id);
        if (salesOrder == null)
        {
            return NotFound();
        }

        _context.SalesOrders.Remove(salesOrder);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SalesOrderExists(int id)
    {
        return _context.SalesOrders.Any(e => e.Id == id);
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetSalesOrderPdf(int id)
    {
        var salesOrder = await _context.SalesOrders
            .Include(o => o.SalesOrderItems)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (salesOrder == null)
        {
            return NotFound();
        }

        // Get tenant branding
        var branding = await GetTenantBranding(salesOrder.TenantId);

        // Generate PDF
        var pdfBytes = _pdfService.GenerateSalesOrderPdf(salesOrder, branding);

        return File(pdfBytes, "application/pdf", $"SalesOrder_{salesOrder.OrderNumber}.pdf");
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
