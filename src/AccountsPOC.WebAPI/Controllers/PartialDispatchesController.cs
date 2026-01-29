using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using AccountsPOC.PdfGenerator.Models;
using AccountsPOC.PdfGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartialDispatchesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPdfGeneratorService _pdfService;

    public PartialDispatchesController(ApplicationDbContext context, IPdfGeneratorService pdfService)
    {
        _context = context;
        _pdfService = pdfService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PartialDispatch>>> GetPartialDispatches()
    {
        return await _context.PartialDispatches
            .Include(d => d.SalesOrder)
            .Include(d => d.PartialDispatchItems)
                .ThenInclude(i => i.SalesOrderItem)
                    .ThenInclude(soi => soi!.Product)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PartialDispatch>> GetPartialDispatch(int id)
    {
        var dispatch = await _context.PartialDispatches
            .Include(d => d.SalesOrder)
            .Include(d => d.PartialDispatchItems)
                .ThenInclude(i => i.SalesOrderItem)
                    .ThenInclude(soi => soi!.Product)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dispatch == null)
        {
            return NotFound();
        }

        return dispatch;
    }

    [HttpPost]
    public async Task<ActionResult<PartialDispatch>> PostPartialDispatch(PartialDispatch dispatch)
    {
        dispatch.CreatedDate = DateTime.UtcNow;
        dispatch.DispatchDate = DateTime.UtcNow;
        
        _context.PartialDispatches.Add(dispatch);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPartialDispatch), new { id = dispatch.Id }, dispatch);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPartialDispatch(int id, PartialDispatch dispatch)
    {
        if (id != dispatch.Id)
        {
            return BadRequest();
        }

        dispatch.LastModifiedDate = DateTime.UtcNow;
        _context.Entry(dispatch).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PartialDispatchExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePartialDispatch(int id)
    {
        var dispatch = await _context.PartialDispatches.FindAsync(id);
        if (dispatch == null)
        {
            return NotFound();
        }

        _context.PartialDispatches.Remove(dispatch);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPartialDispatchPdf(int id)
    {
        var dispatch = await _context.PartialDispatches
            .Include(d => d.SalesOrder)
            .Include(d => d.PartialDispatchItems)
                .ThenInclude(i => i.SalesOrderItem)
                    .ThenInclude(soi => soi!.Product)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dispatch == null)
        {
            return NotFound();
        }

        // Get tenant branding
        var branding = await GetTenantBranding(dispatch.TenantId);

        // Generate PDF
        var pdfBytes = _pdfService.GeneratePartialDispatchPdf(dispatch, branding);

        return File(pdfBytes, "application/pdf", $"Dispatch_{dispatch.DispatchNumber}.pdf");
    }

    private bool PartialDispatchExists(int id)
    {
        return _context.PartialDispatches.Any(e => e.Id == id);
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
