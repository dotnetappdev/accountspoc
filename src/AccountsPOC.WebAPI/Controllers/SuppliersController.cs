using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly int _tenantId = 1; // For POC - in production, get from auth context

    public SuppliersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
    {
        return await _context.Suppliers
            .Where(s => s.TenantId == _tenantId)
            .OrderBy(s => s.SupplierName)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Supplier>> GetSupplier(int id)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == _tenantId);

        if (supplier == null)
        {
            return NotFound();
        }

        return supplier;
    }

    [HttpPost]
    public async Task<ActionResult<Supplier>> CreateSupplier(Supplier supplier)
    {
        supplier.TenantId = _tenantId;
        supplier.CreatedDate = DateTime.UtcNow;

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, supplier);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSupplier(int id, Supplier supplier)
    {
        if (id != supplier.Id)
        {
            return BadRequest();
        }

        var existingSupplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == _tenantId);

        if (existingSupplier == null)
        {
            return NotFound();
        }

        existingSupplier.SupplierCode = supplier.SupplierCode;
        existingSupplier.SupplierName = supplier.SupplierName;
        existingSupplier.ContactName = supplier.ContactName;
        existingSupplier.Email = supplier.Email;
        existingSupplier.Phone = supplier.Phone;
        existingSupplier.Address = supplier.Address;
        existingSupplier.City = supplier.City;
        existingSupplier.PostalCode = supplier.PostalCode;
        existingSupplier.Country = supplier.Country;
        existingSupplier.WebsiteUrl = supplier.WebsiteUrl;
        existingSupplier.ApiEndpoint = supplier.ApiEndpoint;
        existingSupplier.ApiUsername = supplier.ApiUsername;
        existingSupplier.ApiPassword = supplier.ApiPassword;
        existingSupplier.LeadTimeDays = supplier.LeadTimeDays;
        existingSupplier.MinimumOrderValue = supplier.MinimumOrderValue;
        existingSupplier.PaymentTerms = supplier.PaymentTerms;
        existingSupplier.IsActive = supplier.IsActive;
        existingSupplier.LastModifiedDate = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Suppliers.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == _tenantId);

        if (supplier == null)
        {
            return NotFound();
        }

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
